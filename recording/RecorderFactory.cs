public class RecorderFactory {
        public static IRecorder createRecorder() {
                if (System.OperatingSystem.IsLinux()) {
                        return new LinuxRecorder();
                }

                #if OS_WINDOWS
                if (System.OperatingSystem.IsWindows()) {
                        return new WindowsRecorder();
                }
                #endif

                throw new NotSupportedException("This operating system is not currently supported");
        }
}
