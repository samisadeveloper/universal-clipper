using SharpHook.Data;

public class Configuration {
        private string? videosDirectory;
        private KeyCode? hotKey;
        private int clipSizeSeconds = 60;

        private string? inputDevice;
        private string? outputDevice;

        public KeyCode HotKey {
                get => hotKey ?? KeyCode.VcF8;
                set => hotKey = value;
        }

        public string VideosDirectory {
                get => videosDirectory ?? GetDefaultVideoDirectory();
                set => videosDirectory = value;
        }

        public int ClipSizeSeconds {
                get => clipSizeSeconds;
                set => clipSizeSeconds = value;
        }

        public string InputDevice {
                get => inputDevice ?? "default_input";
                set => inputDevice = value;
        }

        public string OutputDevice {
                get => outputDevice ?? "default_output";
                set => outputDevice = value;
        }

        public static string GetDefaultVideoDirectory() {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "universal-clipper");
        }

        public static string getConfigurationDirectory() {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "universal-clipper");
        }

        public void Save() {
                // TODO: use the JSON serializer to serialize the configuration to a file
                // we also need to get the configuration directory
        }

        public static Configuration Load() {
                throw new NotImplementedException();
        }
}
