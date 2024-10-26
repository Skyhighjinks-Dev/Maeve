using MessagePack;

namespace Aorta.Models.Gnomio;
[MessagePackObject]
public class PlayerData
{
  [Key(0)]
  public string Username { get; set; }

  [Key(1)]
  public double X { get; set; }

  [Key(2)]
  public double Y { get; set; }

  [Key(3)]
  public double Z { get; set; }

  public PlayerData()
  { 
  
  }

  [SerializationConstructor]
  public PlayerData(string nUsername, double nX, double nY, double nZ)
  { 
    this.Username = nUsername;
    this.X = nX;
    this.Y = nY;
    this.Z = nZ;
  }
}
