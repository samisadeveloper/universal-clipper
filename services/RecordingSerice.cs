class RecordingService : BackgroundService {
        protected override Task ExecuteAsync(CancellationToken stoppingToken) {
                return Task.Run(() => {
                        var recorder = RecorderFactory.createRecorder();

                        recorder.Start();

                        InputService.defineHotkey(Program.getConfiguration().HotKey, recorder.Save);
                });
        }
}
