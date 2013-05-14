using Anotar.Serilog;

public class ClassWithOldLogging
{
    public void Debug()
    {
        Log.Debug();
    }
}