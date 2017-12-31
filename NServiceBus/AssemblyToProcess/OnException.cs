using System;
using System.Diagnostics;
using Anotar.NServiceBus;

public class OnException
{

    [LogToErrorOnException]
    public void ToError(string param1, int param2)
    {
        throw new Exception("Foo");
    }

    [LogToErrorOnException]
    public object ToErrorWithReturn(string param1, int param2)
    {
        throw new Exception("Foo");
    }
    [LogToFatalOnException]
    public void ToFatal(string param1, int param2)
    {
        throw new Exception("Foo");
    }

    [LogToErrorOnException]
    public object MethodThatReturns(string param1, int param2)
    {
        return "a";
    }
    [LogToFatalOnException]
    public object ToFatalWithReturn(string param1, int param2)
    {
        throw new Exception("Foo");
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
        throw new Exception("Foo");
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
        throw new Exception("Foo");
    }

    [LogToInfoOnException]
    public void ToInfo(string param1, int param2)
    {
        throw new Exception("Foo");
    }

    [LogToInfoOnException]
    public object ToInfoWithReturn(string param1, int param2)
    {
        throw new Exception("Foo");
    }

    [LogToWarnOnException]
    public void ToWarn(string param1, int param2)
    {
        throw new Exception("Foo");
    }

    [LogToWarnOnException]
    public object ToWarnWithReturn(string param1, int param2)
    {
        throw new Exception("Foo");
    }

    [LogToDebugOnException]
    public void ToDebug(string param1, int param2)
    {
        Debug.WriteLine("aString");
        throw new Exception("Foo");
    }

    [LogToDebugOnException]
    public object ToDebugWithReturn(string param1, int param2)
    {
        Debug.WriteLine("aString");
        throw new Exception("Foo");
    }

    //TODO: add tests for these combos. for now it is ok to peVerify it
    [LogToDebugOnException]
    public object ToDebugWithReturnAndTc(string param1, int param2)
    {
        try
        {
            throw new Exception("Foo");
        }
        catch (Exception exception)
        {
            throw new Exception("Foo", exception);
        }
        finally
        {
            Debug.WriteLine("finally");
        }
    }
}