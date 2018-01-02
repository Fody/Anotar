using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Anotar.NLog;
#pragma warning disable 1998

public class ClassWithCompilerGeneratedClasses
{
    public async Task AsyncMethod()
    {
        LogTo.Debug("Foo");
        await Task.Delay(1);
    }

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

    public void AsyncDelegateMethod()
    {
        var action = new Action(async () =>
        {
            LogTo.Debug();
        });
        action();
    }
}