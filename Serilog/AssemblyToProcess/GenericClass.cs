using Anotar.Serilog;
// ReSharper disable UnusedTypeParameter

public class GenericClass<T>
{
    public void Debug()
    {
        LogTo.Debug();
    }
}