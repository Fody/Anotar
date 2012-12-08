using System;
using Anotar;

public class OnException
{

    [LogToErrorOnException]
    public void ToError(string param1, int param2, ref string param3, out string param4)
    {
        throw new Exception("Foo");
    }
    [LogToInfoOnException]
    public void ToInfo(string param1, int param2, ref string param3, out string param4)
    {
        throw new Exception("Foo");
    }
    [LogToTraceOnException]
    public void ToTrace(string param1, int param2, ref string param3, out string param4)
    {
        throw new Exception("Foo");
    }
    [LogToDebugOnException]
    public void ToDebug(string param1, int param2, ref string param3, out string param4)
    {
        throw new Exception("Foo");
    }
}