using System.Diagnostics;
using Anotar.Custom;

public class OnException
{

    [LogToErrorOnException]
    public object MethodThatReturns(string param1, int param2)
    {
        return "a";
    }
    [LogToErrorOnException]
    public void ToError(string param1, int param2)
    {
        throw new("Foo");
    }

    [LogToErrorOnException]
    public object ToErrorWithReturn(string param1, int param2)
    {
        throw new("Foo");
    }
    [LogToFatalOnException]
    public void ToFatal(string param1, int param2)
    {
        throw new("Foo");
    }

    [LogToFatalOnException]
    public object ToFatalWithReturn(string param1, int param2)
    {
        throw new("Foo");
    }

    [LogToErrorOnException]
    public void WithRefs(
        ref string param1,
        ref int param2,
        ref short param3,
        ref long param4,
        ref uint param5,
        ref ushort param6,
        ref ulong param7,
        ref bool param8,
        ref double param9,
        ref decimal param10,
        ref int? param11,
        ref object param12,
        ref char param13,
        ref DateTime param14,
        ref float param15,
        ref IntPtr param16,
        ref ushort param17,
        ref uint param18,
        ref ulong param19,
        ref UIntPtr param20
        )
    {
        throw new("Foo");
    }

    [LogToErrorOnException]
    public object WithRefsWithReturn(
        ref string param1,
        ref int param2,
        ref short param3,
        ref long param4,
        ref uint param5,
        ref ushort param6,
        ref ulong param7,
        ref bool param8,
        ref double param9,
        ref decimal param10,
        ref int? param11,
        ref object param12,
        ref char param13,
        ref DateTime param14,
        ref float param15,
        ref IntPtr param16,
        ref ushort param17,
        ref uint param18,
        ref ulong param19,
        ref UIntPtr param20
        )
    {
        throw new("Foo");
    }

    [LogToInformationOnException]
    public void ToInformation(string param1, int param2)
    {
        throw new("Foo");
    }

    [LogToInformationOnException]
    public object ToInformationWithReturn(string param1, int param2)
    {
        throw new("Foo");
    }

    [LogToWarningOnException]
    public void ToWarning(string param1, int param2)
    {
        throw new("Foo");
    }

    [LogToWarningOnException]
    public object ToWarningWithReturn(string param1, int param2)
    {
        throw new("Foo");
    }

    [LogToDebugOnException]
    public void ToDebug(string param1, int param2)
    {
        Debug.WriteLine("aString");
        throw new("Foo");
    }

    [LogToDebugOnException]
    public object ToDebugWithReturn(string param1, int param2)
    {
        Debug.WriteLine("aString");
        throw new("Foo");
    }

    [LogToTraceOnException]
    public void ToTrace(string param1, int param2)
    {
        throw new("Foo");
    }

    [LogToTraceOnException]
    public object ToTraceWithReturn(string param1, int param2)
    {
        throw new("Foo");
    }

    //TODO: add tests for these combos. for now it is ok to peVerify it
    [LogToDebugOnException]
    public object ToDebugWithReturnAndTc(string param1, int param2)
    {
        try
        {
            throw new("Foo");
        }
        catch (Exception exception)
        {
            throw new("Foo", exception);
        }
        finally
        {
            Debug.WriteLine("finally");
        }
    }
}