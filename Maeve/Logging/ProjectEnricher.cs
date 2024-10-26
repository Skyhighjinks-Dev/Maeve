using Serilog.Core;
using Serilog.Events;

namespace Maeve.Logging;
internal class ProjectEnricher : ILogEventEnricher
{
  /// <summary>
  /// Enriches the Serilog logger
  /// </summary>
  /// <param name="nLogEvent">Raised log event</param>
  /// <param name="nPropertyFactory">Property factory</param>
  public void Enrich(LogEvent nLogEvent, ILogEventPropertyFactory nPropertyFactory)
  {
    /* More or less attempts to provide a project source for each log entry */
    if (nLogEvent.Properties.TryGetValue("SourceContext", out LogEventPropertyValue sourceContext))
    {
      string fullClassName = sourceContext.ToString().Trim('"');
      string projectName = fullClassName.Split('.')[0];
      LogEventProperty projectProperty = nPropertyFactory.CreateProperty("Project", projectName);
      nLogEvent.AddPropertyIfAbsent(projectProperty);
    }
    else
    {
      nLogEvent.AddPropertyIfAbsent(nPropertyFactory.CreateProperty("Project", "Unknown"));
    }
  }
}
