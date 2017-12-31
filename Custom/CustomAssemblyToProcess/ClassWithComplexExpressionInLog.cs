using Anotar.Custom;

public class ClassWithComplexExpressionInLog
{
    object field = null;

    public void Method()
    {
        LogTo.Error(field == null ? "X" : "Y");
    }

}