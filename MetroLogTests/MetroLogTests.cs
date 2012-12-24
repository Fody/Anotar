using System.IO;
using MetroLog;
using NUnit.Framework;

[TestFixture]
public class MetroLogTests:BaseTests
{

    public MetroLogTests()
		: base(Path.GetFullPath(@"..\..\..\AssemblyToProcess\bin\DebugMetroLog\MetroLogAssemblyToProcess.dll"))
    {
        var target = new ActionTarget
        {
            Action = LogEvent
        };

        LogManagerFactory.DefaultConfiguration.AddTarget(LogLevel.Debug, target);
        LogManagerFactory.DefaultConfiguration.AddTarget(LogLevel.Warn, target);
        LogManagerFactory.DefaultConfiguration.AddTarget(LogLevel.Info, target);
        LogManagerFactory.DefaultConfiguration.AddTarget(LogLevel.Error, target);
    }

    void LogEvent(LogEventInfo eventInfo)
    {
        if (eventInfo.Level == LogLevel.Error)
        {
            Errors.Add(eventInfo.Message);
            return;
        }
        if (eventInfo.Level == LogLevel.Warn)
        {
            Warns.Add(eventInfo.Message);
            return;
        }
        if (eventInfo.Level == LogLevel.Info)
        {
            Infos.Add(eventInfo.Message);
            return;
        }
        if (eventInfo.Level == LogLevel.Debug)
        {
            Debugs.Add(eventInfo.Message);
            return;
        }

    }
}