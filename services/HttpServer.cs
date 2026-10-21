using Microsoft.Extensions.FileProviders;

class HttpServer {
        public static void initializeHttpServer(WebApplication application, String videoPath) {
                if (!Directory.Exists(videoPath)) {
                        Directory.CreateDirectory(videoPath);
                }

                application.UseStaticFiles(new StaticFileOptions {
                                FileProvider = new PhysicalFileProvider(videoPath),
                                RequestPath = "/media"
                                });

                application.MapGet("/api/status", () => Results.Ok());

                application.MapGet("/api/getClips", () => {
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
        }
}
