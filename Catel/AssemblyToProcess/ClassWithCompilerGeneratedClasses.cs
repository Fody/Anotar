using Anotar.Catel;
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
        LogTo.Debug();
        yield return 2;
    }

    public void DelegateMethod()
    {
        // ReSharper disable once ConvertClosureToMethodGroup
        var action = () => LogTo.Debug();
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
            LogTo.Info();
        });
        action();
    }
}