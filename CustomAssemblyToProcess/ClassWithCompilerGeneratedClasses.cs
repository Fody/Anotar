using System;
using System.Collections.Generic;
using Anotar.Custom;


public class ClassWithCompilerGeneratedClasses
{
    public async void AsyncMethod()
    {
        Log.Debug("Foo");
    }

    public IEnumerable<int> EnumeratorMethod()
    {
        yield return 1;
        Log.Debug("Foo");
        yield return 2;
    }

    public void DelegateMethod()
    {
        Action action = () => Log.Debug("Foo");
        action();
    }

    public void LambdaMethod()
    {
        var x = 0;
        Action<string> log = l => Log.Debug(l, x);
        log("Foo {0}");
    }
}