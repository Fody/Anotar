using System.Diagnostics;
using Anotar.MetroLog;

public class ClassWithWhileMethod
{

    public void MethodWithWhile()
    {
        while (true)
        {
            Trace.WriteLine("aString");
            break;
        }
        LogTo.Info();
    }
}