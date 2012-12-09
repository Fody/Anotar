using System.IO;
using NUnit.Framework;
using log4net;
using log4net.Config;
using log4net.Core;
using log4net.Repository.Hierarchy;

[TestFixture]
public class Log4NetTests:BaseTests
{

    public Log4NetTests()
		: base(Path.GetFullPath(@"..\..\..\AssemblyToProcess\bin\DebugLog4Net\Log4NetAssemblyToProcess.dll"))
    {
        var hierarchy = (Hierarchy)LogManager.GetRepository();
        hierarchy.Root.RemoveAllAppenders(); /*Remove any other appenders*/

        var target = new ActionAppender
        {
            Action = LogEvent
        };


        BasicConfigurator.Configure(target);
    }
    void LogEvent(LoggingEvent loggingEvent)
    {
        if (loggingEvent.Level == Level.Error)
        {
            errors.Add(loggingEvent.RenderedMessage);
            return;
        }
        if (loggingEvent.Level == Level.Warn)
        {
            warns.Add(loggingEvent.RenderedMessage);
            return;
        }
        if (loggingEvent.Level == Level.Info)
        {
            infos.Add(loggingEvent.RenderedMessage);
            return;
        }
        if (loggingEvent.Level == Level.Debug)
        {
            debugs.Add(loggingEvent.RenderedMessage);
            return;
        }

    }
}