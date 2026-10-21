using SharpHook;
using SharpHook.Data;

class InputService : BackgroundService {
        private struct HotKey {
                public KeyCode Keycode;
                public Action Action;
        };

        private static HotKey? hotkey;

        protected override Task ExecuteAsync(CancellationToken stoppingToken) {
                return Task.Run(() => {
                        var hook = new EventLoopGlobalHook();

                        hook.KeyPressed += (Object? sender, KeyboardHookEventArgs args) => {
                                if (hotkey == null) return; 

                                if (hotkey.Value.Keycode == args.RawEvent.Keyboard.KeyCode) {
                                        hotkey.Value.Action.Invoke();
                                }
                        };

                        hook.Run();
                });
        }

        public static void defineHotkey(SharpHook.Data.KeyCode keycode, Action action) {
                hotkey = new HotKey(){ Keycode = keycode, Action = action };
        }
}
