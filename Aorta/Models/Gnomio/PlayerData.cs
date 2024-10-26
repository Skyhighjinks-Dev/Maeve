using MessagePack;

namespace Aorta.Models.Gnomio;
[MessagePackObject]
public class PlayerData
{
  /// <summary>MC Username</summary>
  [Key(0)]
  public string Username { get; set; }

  /// <summary>MC Location: X</summary>
  [Key(1)]
  public double X { get; set; }

  /// <summary>MC Location: Y</summary>
  [Key(2)]
  public double Y { get; set; }

  /// <summary>MC Location: Z</summary>
  [Key(3)]
  public double Z { get; set; }


  /// <summary>
  /// Empty constructor
  /// </summary>
  public PlayerData()
  { 
  
  }


  /// <summary>
  /// MessagePack constructor
  /// </summary>
  /// <param name="nUsername">Username</param>
  /// <param name="nX">X Pos</param>
  /// <param name="nY">Y Pos</param>
  /// <param name="nZ">Z Pos</param>
  [SerializationConstructor]
  public PlayerData(string nUsername, double nX, double nY, double nZ)
  { 
    this.Username = nUsername;
    this.X = nX;
    this.Y = nY;
    this.Z = nZ;
  }
}
