using System;
using System.Collections.Generic;
using Anotar.Custom;
// ReSharper disable UnusedMember.Global
#pragma warning disable 1998


public class ClassWithCompilerGeneratedClasses
{
#if !NET35
    public async void AsyncMethod()
    {
        LogTo.Debug("Foo");
    }
#endif

    public IEnumerable<int> EnumeratorMethod()
    {
        yield return 1;
        LogTo.Debug("Foo");
        yield return 2;
    }

    public void DelegateMethod()
    {
        Action action = () => LogTo.Debug("Foo");
        action();
    }

    public void LambdaMethod()
    {
        var x = 0;
        Action<string> log = l => LogTo.Debug(l, x);
        log("Foo {0}");
    }

#if !NET35
    public void AsyncDelegateMethod()
    {
        var action = new Action(async () =>
        {
            LogTo.Debug();
        });
        action();
    }
#endif
}