using System.Diagnostics;
using Anotar.NLog;

public class ClassWithWhileMethod
{
    public void MethodWithWhile()
    {
        while (true)
        {
            Trace.WriteLine("dfg");
            break;
        }
        Log.Info();
    }
}