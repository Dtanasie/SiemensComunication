using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using SiemensComunication;

class Program
{
    static async Task Main(string[] args)
    {
        // Configure Serilog
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        GlobalLogger.InitializeLogger(); // Setează loggerul global

        try
        {
            Log.Information("Starting application...");
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<Worker>();
                    services.Configure<PLCSettings>(context.Configuration.GetSection("PLC"));
                    services.Configure<FolderSettings>(context.Configuration.GetSection("Folders"));
                })
                .UseSerilog() // Use Serilog for logging
                .Build();

            var worker = host.Services.GetRequiredService<Worker>();
            await worker.RunAsync();
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
