using Microsoft.Extensions.FileProviders;
using SharpHook;

public class Program {
        private static Configuration? configuration;

        public static void Main() {
                configuration = new Configuration();

                // TODO: move this too
                Task.Run(() => {
                        IRecorder recorder = RecorderFactory.createRecorder();

                        recorder.Start();

                        var hook = new EventLoopGlobalHook();

                        hook.KeyPressed += (Object? sender, KeyboardHookEventArgs args) => {
                                if (args.RawEvent.Keyboard.KeyCode == configuration.HotKey) {
                                        recorder.Save();
                                }
                        };

                        hook.Run();
                });

                // TODO: move all of this later please
                var builder = WebApplication.CreateBuilder();

                // TODO: CHANGE THIS LATER PLEASe
                builder.Services.AddCors(options => {
                        options.AddPolicy("AllowEverything", policy => {
                                policy.AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader();
                        });
                });

                var app = builder.Build();

                app.UseCors("AllowEverything");

                String videoPath = configuration.VideosDirectory;

                if (!Directory.Exists(videoPath)) {
                        Directory.CreateDirectory(videoPath);
                }

                app.UseStaticFiles(new StaticFileOptions {
                                FileProvider = new PhysicalFileProvider(videoPath),
                                RequestPath = "/media"
                                });

                app.MapGet("/api/status", () => Results.Ok());

                app.MapGet("/api/getClips", () => {
                                String[] files = Directory.GetFiles(videoPath);

                                String[] clips = files
                                .Select(Path.GetFileName)
                                .Where(fileName => fileName.EndsWith("mp4"))
                                .Select(fileName => {
                                                return fileName.Split(".mp4")[0];
                                                })
                                .ToArray();

                                return Results.Ok(clips);
                                });

                app.Run();
        }

        public static Configuration getConfiguration() {
                if (configuration == null) throw new Exception("Configuration not yet loaded!");

                return configuration;
        }
}
