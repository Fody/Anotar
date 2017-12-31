using Anotar.Serilog;
using Serilog;
using Log = Serilog.Log;

public class ClassWithExistingField
{
// ReSharper disable once NotAccessedField.Local
    static ILogger existingLogger;

    static ClassWithExistingField()
    {
        existingLogger = Log.ForContext<ClassWithExistingField>();
    }

    public void Debug()
    {
        LogTo.Debug();
    }

}