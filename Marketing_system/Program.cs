using Marketing_system.BL.Contracts.IService;
using Marketing_system.BL.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;

namespace Marketing_system
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;

                try
                {
                    var alertService = services.GetRequiredService<IAlertService>();

                    Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Debug()
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                        .Enrich.FromLogContext()
                        .WriteTo.Console()
                        .WriteTo.Async(a => a.Sink(new SignalRSink(alertService)))
                        .WriteTo.File(new JsonFormatter(), "logs/log-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 50)
                        .CreateLogger();

                    Log.Information("Starting up the service");
                    await host.RunAsync();
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex, "Application start-up failed");
                }
                finally
                {
                    Log.CloseAndFlush();
                }
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseSerilog(); // Ovo osigurava da se koristi Serilog tokom pokretanja
    }
}
