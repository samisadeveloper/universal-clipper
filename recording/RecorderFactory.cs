public class RecorderFactory {
        public static IRecorder createRecorder() {
                if (System.OperatingSystem.IsLinux()) {
                        return new LinuxRecorder();
                }

                if (System.OperatingSystem.IsWindows()) {
                        return new WindowsRecorder();
                }

                throw new NotSupportedException("This operating system is not currently supported");
        }
}
