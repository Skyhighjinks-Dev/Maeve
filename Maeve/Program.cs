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
    /// <summary>Host for dependency injection</summary>
    private static IHost _Host { get; set; }

    /// <summary>UDP Service instance (from DI)</summary>
    public static Server _Server { get { return _Host.Services.GetRequiredService<Server>(); } }


    /// <summary>
    /// Main method
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    static async Task Main(string[] args)
    {
      SetupDependencies(args);

      // Retrieve the logger
      var logger = _Host.Services.GetRequiredService<ILogger<Program>>();

      logger.LogInformation("Application Starting");

      // Gets the Server object
      Server server = _Host.Services.GetRequiredService<Server>();
      
      /* Task list to run parrallel */
      List<Task> taskLst = new List<Task>();
      taskLst.Add(Task.Run(async () => await server.StartAsync()));
      taskLst.Add(Task.Run(async () =>
      { 
        /* Goes around and prints data - will revise */
        bool loggedNoPlayerData = false;
        while(true)
        { 
          // Logs data every half second IF data has been logged previously
          // Otherwise attempts to try every 5 seconds
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


    /// <summary>
    /// Dependency injection builder 
    /// </summary>
    /// <param name="nArgs">¯\_(ツ)_/¯</param>
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
