using System.Diagnostics;
using Anotar.Catel;

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