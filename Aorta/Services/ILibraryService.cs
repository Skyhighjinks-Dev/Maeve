namespace Aorta.Services;

/// <summary>
/// Library service to run library tasks from Maeve
/// </summary>
public interface ILibraryService
{
  void Start();
  Task StartAsync();
}
