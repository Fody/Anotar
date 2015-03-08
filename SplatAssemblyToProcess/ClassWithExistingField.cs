using Anotar.Splat;
using Splat;

public class ClassWithExistingField
{
// ReSharper disable once NotAccessedField.Local
    static IFullLogger existingLogger;

    static ClassWithExistingField()
    {
        existingLogger = Locator.Current.GetService<ILogManager>().GetLogger(typeof(ClassWithExistingField));
        
    }

    public void Debug()
    {
        LogTo.Debug();
    }
    public void Foo()
    {
        var infoEnabled = existingLogger.Level >= LogLevel.Error;
    }
    public void Foo2()
    {
        var infoEnabled = LogTo.IsInfoEnabled;
    }
}