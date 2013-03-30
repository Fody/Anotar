using Serilog;
using Serilog.Events;
using Log = Serilog.Log;

public class ClassWithExistingField
{
    static ILogger existingLogger;

    static ClassWithExistingField()
    {
		existingLogger = Log.ForContext<ClassWithExistingField>();
    }

    public void Debug()
    {
		if (existingLogger.IsEnabled(LogEventLevel.Debug))
		{
			Anotar.Log.Debug();
		}
    }
}