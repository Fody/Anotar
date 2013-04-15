using System.Diagnostics;
using Anotar.Serilog;

public class ClassWithWhileMethod
{

    public void MethodWithWhile()
    {
        while (true)
        {
            Trace.WriteLine("dfg");
            break;
        }
        Log.Information();
    }
}