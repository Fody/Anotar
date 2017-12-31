using Anotar.NServiceBus;

public class ClassWithComplexExpressionInLog
{
    object field = null;

    public void Method()
    {
        LogTo.Error(field == null ? "X" : "Y");
    }
}