#if OS_WINDOWS
using ScreenRecorderLib;

class WindowsRecorder : IRecorder {
        Stream stream = Stream.Null;
        Recorder? recorder;

        public void Start() {
                RecorderOptions options = new RecorderOptions {};

                recorder = Recorder.CreateRecorder();

                string fileName = Guid.NewGuid().ToString() + ".mp4";

                // lets just write to a file now until we find a real solution
                recorder.Record(Path.Combine(Program.getConfiguration().VideosDirectory, fileName));

                // TODO: find a way to record in a buffer
                // record to a stream and instead of ending the recording just switch between both streams (possibly)
                // recorder.Record(stream);
        }

        public void End() {
                throw new NotImplementedException();
        }

        public void Save() {
                if (recorder == null) throw new Exception("Recording has not started!");

                recorder.Stop();

                Console.WriteLine("Saved a recording");
        }

}
#endif
