using SharpHook.Data;

public class Configuration {
        private String? videosDirectory;
        private KeyCode? hotKey;
        private int clipSizeSeconds = 60;

        private String? inputDevice;
        private String? outputDevice;

        public KeyCode HotKey {
                get => hotKey ?? KeyCode.VcF8;
                set => hotKey = value;
        }

        public String VideosDirectory {
                get => videosDirectory ?? GetDefaultVideoDirectory();
                set => videosDirectory = value;
        }

        public int ClipSizeSeconds {
                get => clipSizeSeconds;
                set => clipSizeSeconds = value;
        }

        public String InputDevice {
                get => inputDevice ?? "default_input";
                set => inputDevice = value;
        }

        public String OutputDevice {
                get => outputDevice ?? "default_output";
                set => outputDevice = value;
        }

        public static String GetDefaultVideoDirectory() {
                if (System.OperatingSystem.IsLinux()) {
                        String userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

                        return Path.Combine(userProfile, "Videos", "universal-clipper");
                }

                throw new NotSupportedException("This operating system is not currently supported");
        }

        public static String getConfigurationDirectory() {
                if (System.OperatingSystem.IsLinux()) {
                        // do stuff
                        // not the user profile btw
                }

                throw new NotSupportedException("This operating system is not currently supported");
        }

        public void Save() {
                // TODO: use the JSON serializer to serialize the configuration to a file
                // we also need to get the configuration directory
        }

        public static Configuration Load() {
                throw new NotImplementedException();
        }
}
