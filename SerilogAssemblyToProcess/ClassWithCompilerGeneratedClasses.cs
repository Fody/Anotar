using System;
using System.Collections.Generic;
using Anotar.Serilog;


public class ClassWithCompilerGeneratedClasses
{
    public async void AsyncMethod()
    {
        LogTo.Debug();
    }

    public IEnumerable<int> EnumeratorMethod()
    {
        yield return 1;
        LogTo.Debug();
        yield return 2;
    }

    public void DelegateMethod()
    {
        // ReSharper disable once ConvertClosureToMethodGroup
        Action action = () => LogTo.Debug();
        action();
    }

    public void LambdaMethod()
    {
        var x = 0;
        Action<string> log = l => LogTo.Debug(l, x);
        log("Foo {0}");
    }
}