using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Maeve.Logging;
using Axon.Gnomio;
using Aorta.Models.Gnomio;

namespace Maeve
{
  class Program
  {
    private static IHost _Host { get; set; }
    public static Server _Server { get { return _Host.Services.GetRequiredService<Server>(); } }

    static async Task Main(string[] args)
    {
      SetupDependencies(args);

      // Retrieve the logger
      var logger = _Host.Services.GetRequiredService<ILogger<Program>>();

      logger.LogInformation("Application Starting");

      Server server = _Host.Services.GetRequiredService<Server>();
      
      List<Task> taskLst = new List<Task>();
      taskLst.Add(Task.Run(async () => await server.StartAsync()));
      taskLst.Add(Task.Run(async () =>
      { 
        bool loggedNoPlayerData = false;
        while(true)
        { 
          await Task.Delay(500 * (loggedNoPlayerData ? 10 : 1));

          if (server.PlayerData.IsEmpty)
          {
            logger.LogInformation("No player data available.");
            loggedNoPlayerData = true;
            continue;
          }

          loggedNoPlayerData = false;

          foreach (var kvp in server.PlayerData)
          {
            string username = kvp.Key;
            PlayerData data = kvp.Value;

            logger.LogInformation($"Player: {username}, X: {data.X}, Y: {data.Y}, Z: {data.Z}");
          }
        }
      }));

      await server.StartAsync();

      logger.LogInformation("Application Ending");
    }

    private static void SetupDependencies(string[] nArgs)
    {
      _Host = Host.CreateDefaultBuilder(nArgs)
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
          config.AddJsonFile("serilog.json", optional: false, reloadOnChange: true);
        })
        .UseSerilog((hostingContext, services, loggerConfiguration) =>
        {
          loggerConfiguration
                    .ReadFrom.Configuration(hostingContext.Configuration)
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.WithThreadId()
                    .Enrich.With(new ProjectEnricher());
        })
        .ConfigureServices((context, services) =>
        { 
          services.AddTransient<Server>();
        })
        .Build();
    }
  }
}
