using Serilog.Core;
using Serilog.Events;
using System.Reflection;

namespace Maeve.Logging;
internal class ProjectEnricher : ILogEventEnricher
{
  public void Enrich(LogEvent nLogEvent, ILogEventPropertyFactory nPropertyFactory)
  {
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
