class WindowsRecorder : IRecorder {
        public void Start() {
                Console.WriteLine("Starting a recorder");
        }

        public void End() {
                throw new NotImplementedException();
        }

        public void Save() {
                Console.WriteLine("Saving a recording");
        }

}
