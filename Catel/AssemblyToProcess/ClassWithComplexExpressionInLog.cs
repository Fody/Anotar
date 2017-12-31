using Anotar.Catel;

public class ClassWithComplexExpressionInLog
{
    object field = null;

    public void Method()
    {
        LogTo.Error(field == null ? "X" : "Y");
    }
}