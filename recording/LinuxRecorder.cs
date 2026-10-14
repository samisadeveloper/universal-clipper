using System.Diagnostics;
using System.Runtime.InteropServices;

public class LinuxRecorder : IRecorder {
        [DllImport("libc", SetLastError = true)]
        private static extern int kill(int pid, int sig);

        private Process? process;

        public void Start() {
                Configuration configuration = Program.getConfiguration();

                var psi = new ProcessStartInfo("gpu-screen-recorder") {
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                };

                psi.ArgumentList.Add("-r"); psi.ArgumentList.Add(configuration.ClipSizeSeconds.ToString());
                psi.ArgumentList.Add("-c"); psi.ArgumentList.Add("mp4");
                psi.ArgumentList.Add("-w"); psi.ArgumentList.Add("screen");
                psi.ArgumentList.Add("-o"); psi.ArgumentList.Add(configuration.VideosDirectory);
                psi.ArgumentList.Add("-a"); psi.ArgumentList.Add($"{configuration.InputDevice}|{configuration.OutputDevice}");

                process = Process.Start(psi) ?? throw new InvalidOperationException("Failed to start");
        }

        public void End() {
                process?.Kill();
        }

        public void Save() {
                if (process == null) throw new Exception("Clipping backend not found!");

                kill(process.Id, 10);
        }

}
