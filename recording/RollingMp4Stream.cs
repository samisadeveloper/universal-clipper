#if OS_WINDOWS
using System.Buffers.Binary;
using System.Diagnostics;

namespace universal_clipper_client;

/// <summary>
/// Write-only stream that accepts fragmented MP4 output, splits it into
/// top-level boxes, and keeps only the init segment plus the last N seconds
/// of fragments (moof+mdat) in RAM.
/// </summary>
public sealed class RollingMp4Stream : Stream
{
    private readonly object _lock = new();
    private readonly long _windowTicks;

    // Parse buffer (byte[] with start/end offsets, no per-box RemoveRange copies)
    private byte[] _buf = new byte[1 << 20];
    private int _start, _end;

    // Init segment (ftyp + moov), kept forever
    private readonly MemoryStream _init = new();
    private bool _sawFragment;

    // Fragments (moof + mdat), rolling
    private byte[]? _moof;
    private readonly Queue<(long t, byte[] data, int[] baseFields, int[] trunFields)> _frags = new();
    private long _fragBytes;

    // Fake position bookkeeping so the Media Foundation sink is happy with CanSeek
    private long _length, _position;

    /// <summary>Writes that landed anywhere but the end. Should stay 0 for fragmented MP4.</summary>
    public long NonAppendWrites { get; private set; }
    public long BufferedBytes { get { lock (_lock) return _init.Length + _fragBytes; } }
    public int FragmentCount { get { lock (_lock) return _frags.Count; } }

    public RollingMp4Stream(TimeSpan window)
        => _windowTicks = (long)(window.TotalSeconds * Stopwatch.Frequency);

    // ---------- Snapshotting ----------

    /// <summary>Returns a complete, playable MP4 of the current window.</summary>
    public byte[] Snapshot()
    {
        using var ms = new MemoryStream();
        foreach (var part in BuildOutput()) ms.Write(part);
        return ms.ToArray();
    }

    /// <summary>Writes the current window straight to disk.</summary>
    public void SaveSnapshot(string path)
    {
        using var fs = File.Create(path);
        foreach (var part in BuildOutput()) fs.Write(part);
    }

    private IEnumerable<byte[]> BuildOutput()
    {
        byte[] init;
        (long t, byte[] data, int[] baseFields, int[] trunFields)[] frags;
        lock (_lock)
        {
            init = _init.ToArray();
            frags = _frags.ToArray();
        }

        yield return init;
        long pos = init.Length;

        foreach (var f in frags)
        {
            var data = f.data;
            if (f.baseFields.Length > 0 || f.trunFields.Length > 0)
            {
                data = (byte[])data.Clone();

                // tfhd base_data_offset is an absolute file offset: point it at where this moof now lives.
                foreach (int off in f.baseFields)
                    BinaryPrimitives.WriteUInt64BigEndian(data.AsSpan(off), (ulong)pos);

                // trun data_offset is relative to the moof start. Our fragment is moof immediately
                // followed by mdat, so the first sample sits at moofSize + 8 (the mdat header).
                int moofSize = (int)BinaryPrimitives.ReadUInt32BigEndian(data);
                foreach (int off in f.trunFields)
                    BinaryPrimitives.WriteInt32BigEndian(data.AsSpan(off), moofSize + 8);
            }
            yield return data;
            pos += data.Length;
        }
    }

    private static bool IsType(byte[] b, int i, ReadOnlySpan<byte> t) => b.AsSpan(i, 4).SequenceEqual(t);

    /// <summary>
    /// Locates the tfhd base_data_offset fields and the first trun's data_offset field
    /// (first track only) inside a moof box, as byte offsets into the array.
    /// </summary>
    private static (int[] baseFields, int[] trunFields) FindPatchFields(byte[] moof)
    {
        var bases = new List<int>();
        var truns = new List<int>();
        bool firstTraf = true;
        int p = 8; // skip moof header
        while (p + 8 <= moof.Length)
        {
            uint sz = BinaryPrimitives.ReadUInt32BigEndian(moof.AsSpan(p));
            if (sz < 8 || p + sz > moof.Length) break;

            if (IsType(moof, p + 4, "traf"u8))
            {
                int q = p + 8, end = p + (int)sz;
                bool firstTrun = true;
                while (q + 8 <= end)
                {
                    uint csz = BinaryPrimitives.ReadUInt32BigEndian(moof.AsSpan(q));
                    if (csz < 8 || q + csz > end) break;

                    if (IsType(moof, q + 4, "tfhd"u8))
                    {
                        uint flags = BinaryPrimitives.ReadUInt32BigEndian(moof.AsSpan(q + 8)) & 0xFFFFFF;
                        if ((flags & 0x1) != 0) bases.Add(q + 16); // size/type/ver+flags/trackID, then base offset
                    }
                    else if (firstTraf && firstTrun && IsType(moof, q + 4, "trun"u8))
                    {
                        uint flags = BinaryPrimitives.ReadUInt32BigEndian(moof.AsSpan(q + 8)) & 0xFFFFFF;
                        if ((flags & 0x1) != 0) truns.Add(q + 16); // size/type/ver+flags/sampleCount, then data_offset
                        firstTrun = false;
                    }
                    q += (int)csz;
                }
                firstTraf = false;
            }
            p += (int)sz;
        }
        return (bases.ToArray(), truns.ToArray());
    }

    /// <summary>Human-readable dump of the oldest buffered fragment's original (unpatched) offsets.</summary>
    public string Describe()
    {
        lock (_lock)
        {
            if (_frags.Count == 0) return $"no fragments yet (init={_init.Length}B, nonAppend={NonAppendWrites})";
            var f = _frags.Peek();
            uint moofSize = BinaryPrimitives.ReadUInt32BigEndian(f.data);
            string baseOff = f.baseFields.Length > 0
                ? BinaryPrimitives.ReadUInt64BigEndian(f.data.AsSpan(f.baseFields[0])).ToString() : "absent";
            string dataOff = f.trunFields.Length > 0
                ? BinaryPrimitives.ReadInt32BigEndian(f.data.AsSpan(f.trunFields[0])).ToString() : "absent";
            string count = f.trunFields.Length > 0
                ? BinaryPrimitives.ReadUInt32BigEndian(f.data.AsSpan(f.trunFields[0] - 4)).ToString() : "?";
            return $"frags={_frags.Count} init={_init.Length}B moof={moofSize}B fragLen={f.data.Length}B " +
                   $"origBaseOffset={baseOff} origTrunDataOffset={dataOff} (expected {moofSize + 8}) samples={count} nonAppend={NonAppendWrites}";
        }
    }

    // ---------- Stream write path ----------

    public override void Write(byte[] buffer, int offset, int count)
        => Write(new ReadOnlySpan<byte>(buffer, offset, count));

    public override void Write(ReadOnlySpan<byte> data)
    {
        if (data.IsEmpty) return;

        lock (_lock)
        {
            if (_position != _length)
            {
                // Sink seeked somewhere and rewrote bytes. We can't patch history; just count it.
                NonAppendWrites++;
                _position += data.Length;
                if (_position > _length) _length = _position;
                return;
            }

            Append(data);
            _position += data.Length;
            _length = _position;
            ParseBoxes();
        }
    }

    private void Append(ReadOnlySpan<byte> data)
    {
        if (_buf.Length - _end < data.Length)
        {
            // Compact first
            int live = _end - _start;
            if (_start > 0)
            {
                Buffer.BlockCopy(_buf, _start, _buf, 0, live);
                _start = 0;
                _end = live;
            }
            if (_buf.Length - _end < data.Length)
                Array.Resize(ref _buf, Math.Max(_buf.Length * 2, _end + data.Length));
        }
        data.CopyTo(_buf.AsSpan(_end));
        _end += data.Length;
    }

    private void ParseBoxes()
    {
        while (_end - _start >= 8)
        {
            var span = _buf.AsSpan(_start, _end - _start);
            long size = BinaryPrimitives.ReadUInt32BigEndian(span);

            if (size == 1) // 64-bit largesize
            {
                if (span.Length < 16) break;
                size = (long)BinaryPrimitives.ReadUInt64BigEndian(span[8..]);
            }

            if (size < 8 || size > int.MaxValue)
            {
                // Garbage or unsupported (size 0 = "to EOF"). Drop what we have and resync on next write.
                _start = _end = 0;
                return;
            }

            if (span.Length < size) break; // wait for the rest of the box

            string type = System.Text.Encoding.ASCII.GetString(span.Slice(4, 4));
            byte[] box = span[..(int)size].ToArray();
            _start += (int)size;

            HandleBox(type, box);
        }

        if (_start == _end) _start = _end = 0;
    }

    private void HandleBox(string type, byte[] box)
    {
        switch (type)
        {
            case "ftyp":
            case "moov":
                if (!_sawFragment) _init.Write(box);
                break;

            case "moof":
                _sawFragment = true;
                _moof = box;
                break;

            case "mdat" when _moof != null:
                var frag = new byte[_moof.Length + box.Length];
                Buffer.BlockCopy(_moof, 0, frag, 0, _moof.Length);
                Buffer.BlockCopy(box, 0, frag, _moof.Length, box.Length);
                _moof = null;

                long now = Stopwatch.GetTimestamp();
                var (bf, tf) = FindPatchFields(frag);
                _frags.Enqueue((now, frag, bf, tf));
                _fragBytes += frag.Length;

                while (_frags.Count > 1 && now - _frags.Peek().t > _windowTicks)
                    _fragBytes -= _frags.Dequeue().data.Length;
                break;

            // styp / sidx / mfra / free / etc. are ignored
        }
    }

    // ---------- Stream plumbing ----------

    public override bool CanRead => false;
    public override bool CanSeek => true;
    public override bool CanWrite => true;

    public override long Length { get { lock (_lock) return _length; } }

    public override long Position
    {
        get { lock (_lock) return _position; }
        set { lock (_lock) _position = value; }
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        lock (_lock)
        {
            _position = origin switch
            {
                SeekOrigin.Begin => offset,
                SeekOrigin.Current => _position + offset,
                SeekOrigin.End => _length + offset,
                _ => throw new ArgumentOutOfRangeException(nameof(origin))
            };
            return _position;
        }
    }

    public override void SetLength(long value) { }
    public override void Flush() { }
    public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
}
#endif
