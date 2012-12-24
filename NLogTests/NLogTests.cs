using System.IO;
using NLog;
using NLog.Config;
using NUnit.Framework;

[TestFixture]
public class NLogTests:BaseTests
{

    public NLogTests()
        : base(Path.GetFullPath(@"..\..\..\AssemblyToProcess\bin\DebugNlog\NLogAssemblyToProcess.dll"))
    {
        var config = new LoggingConfiguration();
        var target = new ActionTarget
            {
                Action = LogEvent
            };

        config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));
        config.AddTarget("debuger", target);
        LogManager.Configuration = config;
    }

    void LogEvent(LogEventInfo eventInfo)
    {
        if (eventInfo.Level == LogLevel.Error)
        {
            Errors.Add(eventInfo.FormattedMessage);
            return;
        }        
        if (eventInfo.Level == LogLevel.Warn)
        {
            Warns.Add(eventInfo.FormattedMessage);
            return;
        }        
        if (eventInfo.Level == LogLevel.Info)
        {
            Infos.Add(eventInfo.FormattedMessage);
            return;
        }
        if (eventInfo.Level == LogLevel.Debug)
        {
            Debugs.Add(eventInfo.FormattedMessage);
            return;
        }        

    }
}