public class RecorderFactory {
        public static IRecorder createRecorder() {
                if (System.OperatingSystem.IsLinux()) {
                        return new LinuxRecorder();
                }

                throw new NotSupportedException("This opeating system is not currently supported");
        }
}
