
public class Program {
        private static Configuration? configuration;

        public static void Main() {
                configuration = new Configuration();

                WebApplicationBuilder builder = WebApplication.CreateBuilder();

                // WARNING: insecure
                builder.Services.AddCors(options => {
                        options.AddPolicy("AllowEverything", policy => {
                                policy.AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader();
                        });
                });

                builder.Services.AddHostedService<InputService>();
                builder.Services.AddHostedService<RecordingService>();

                var app = builder.Build();

                // WARNING: insecure
                app.UseCors("AllowEverything");
                
                HttpServer.initializeHttpServer(app, configuration.VideosDirectory);

                app.Run();
        }

        public static Configuration getConfiguration() {
                if (configuration == null) throw new Exception("Configuration not yet loaded!");

                return configuration;
        }
}
