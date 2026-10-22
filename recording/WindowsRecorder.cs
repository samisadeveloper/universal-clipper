#if OS_WINDOWS
using ScreenRecorderLib;
using universal_clipper_client;

class WindowsRecorder : IRecorder {
        RollingMp4Stream? buffer;
        Recorder? recorder;

        public void Start() {
                RecorderOptions options = new RecorderOptions {
                        VideoEncoderOptions = new VideoEncoderOptions
                        {
                                IsFragmentedMp4Enabled = true,
                                IsHardwareEncodingEnabled = true,
                        }
                };

                buffer = new RollingMp4Stream(TimeSpan.FromSeconds(Program.getConfiguration().ClipSizeSeconds));

                recorder = Recorder.CreateRecorder(options);

                recorder.Record(buffer);
        }

        public void End() {
                throw new NotImplementedException();
        }

        public void Save() {
                if (recorder == null) throw new Exception("Recording has not started!");
                if (buffer == null) throw new Exception("No buffer found!");

                string fileName = Guid.NewGuid().ToString() + ".mp4";
                string path = Program.getConfiguration().VideosDirectory;

                buffer?.SaveSnapshot(Path.Combine(path, fileName));
                Console.WriteLine("Saved a recording");
        }

}
#endif
