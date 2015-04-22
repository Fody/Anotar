using Anotar.Splat;
using Splat;

public class ClassWithExistingField
{
// ReSharper disable once NotAccessedField.Local
    static IFullLogger existingLogger;

    static ClassWithExistingField()
    {
        existingLogger = Locator.Current.GetService<ILogManager>()
            .GetLogger(typeof (ClassWithExistingField));
    }

    public void Debug()
    {
        LogTo.Debug();
    }
}