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
            Errors.Add(loggingEvent.RenderedMessage);
            return;
        }
        if (loggingEvent.Level == Level.Warn)
        {
            Warns.Add(loggingEvent.RenderedMessage);
            return;
        }
        if (loggingEvent.Level == Level.Info)
        {
            Infos.Add(loggingEvent.RenderedMessage);
            return;
        }
        if (loggingEvent.Level == Level.Debug)
        {
            Debugs.Add(loggingEvent.RenderedMessage);
            return;
        }

    }
}