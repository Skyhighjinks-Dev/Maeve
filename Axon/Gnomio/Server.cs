using Aorta.Models.Gnomio;
using Aorta.Services;
using MessagePack;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace Axon.Gnomio;
public class Server : ILibraryService
{
  private readonly ILogger Logger;
  public ConcurrentDictionary<string, PlayerData> PlayerData = new ConcurrentDictionary<string, PlayerData>();

  public Server(ILogger<Server> nLogger)
  {
    Logger = nLogger;
  }

  public void Start()
  {
    throw new NotImplementedException();
  }

  public async Task StartAsync()
  {
    Logger.LogInformation("Started server");
    
    const int port = 6969;
    using(UdpClient client = new UdpClient(port))
    {
      Logger.LogInformation($"UDP Server listening on port {port}");

      while(true)
      {
        try
        {
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

  private void HandleReceivedData(UdpReceiveResult nResult)
  { 
    try
    { 
      Logger.LogInformation("Data received");
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
