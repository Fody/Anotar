using System;
using Anotar.CommonLogging;

public class ClassWithWhileMethod
{

    public void MethodWithWhile()
    {
        while (true)
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            TimeSpan.FromDays(1);
            break;
        }
        LogTo.Info();
    }
}