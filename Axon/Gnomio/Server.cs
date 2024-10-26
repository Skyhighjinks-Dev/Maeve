using Aorta.Models.Gnomio;
using Aorta.Services;
using MessagePack;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace Axon.Gnomio;
public class Server : ILibraryService
{
  /// <summary>Logger (handled via dependency injection)</summary>
  private readonly ILogger Logger;

  /// <summary>Thread safe dictionary for player data</summary>
  public ConcurrentDictionary<string, PlayerData> PlayerData = new ConcurrentDictionary<string, PlayerData>();


  /// <summary>
  /// Constructor
  /// </summary>
  /// <param name="nLogger">Logger passed via dependency injection</param>
  public Server(ILogger<Server> nLogger)
  {
    Logger = nLogger;
  }


  /// <summary>
  /// Start method - not used
  /// </summary>
  /// <exception cref="NotImplementedException">not used</exception>
  public void Start()
  {
    throw new NotImplementedException();
  }


  /// <summary>
  /// Starts the UDP server
  /// </summary>
  /// <returns></returns>
  public async Task StartAsync()
  {
    Logger.LogInformation("Started server");
    
    // Uses hardcoded port, might be worth making it adjustable
    const int port = 6969;

    // Created UDP Server
    using(UdpClient client = new UdpClient(port))
    {
      Logger.LogInformation($"UDP Server listening on port {port}");

      while(true)
      {
        try
        {
          /* Accept UDP data and handles it accordingly */
          UdpReceiveResult result = await client.ReceiveAsync();
          _ = Task.Run(() => HandleReceivedData(result));
        }
        catch (Exception ex)
        {
          Logger.LogError(ex, "Error receiving UDP data");
        }
      }
    }
  }


  /// <summary>
  /// Handles data received from the UDP port
  /// </summary>
  /// <param name="nResult"></param>
  private void HandleReceivedData(UdpReceiveResult nResult)
  { 
    try
    { 
      Logger.LogInformation("Data received");

      /* Converts the data to a PlayerData object
         TODO: Redo the payload system */
      byte[] data = nResult.Buffer;
      PlayerData playerData = MessagePackSerializer.Deserialize<PlayerData>(data);

      // Validate the deserialized object
      if (playerData == null)
      {
        Logger.LogWarning("Deserialized PlayerData is null.");
        return;
      }

      if (string.IsNullOrWhiteSpace(playerData.Username))
      {
        Logger.LogWarning("Received PlayerData with empty or null Username.");
        return;
      }

      // Attempts to add or update the current data held within the thread-safe dictionary
      PlayerData.AddOrUpdate(
          playerData.Username,              // Key: Username
          playerData,                       // Value to add if key does not exist
          (key, existingVal) => playerData  // Update value if key exists
      );

      Logger.LogInformation($"PlayerData for '{playerData.Username}' added/updated successfully.");
      Logger.LogDebug($"PlayerData Details: {playerData.Username}: {playerData.X}, {playerData.Y}, {playerData.Z}");
    }
    catch (MessagePackSerializationException ex)
    {
      Logger.LogError(ex, "Failed to deserialize PlayerData. The data may be corrupted or improperly formatted.");
    }
    catch (Exception ex)
    {
      Logger.LogError(ex, "An unexpected error occurred while handling received data.");
    }
  }
}
