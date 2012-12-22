using System;
using System.Diagnostics;
using Anotar;

public class OnException
{

    [LogToErrorOnException]
    public void ToError(string param1, int param2)
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
        ref Single param15,
        ref IntPtr param16,
        ref UInt16 param17,
        ref UInt32 param18,
        ref UInt64 param19,
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
    [LogToWarnOnException]
    public void ToWarn(string param1, int param2)
    {

        throw new Exception("Foo");
    }
    [LogToDebugOnException]
    public void ToDebug(string param1, int param2)
    {
        Debug.WriteLine("sdf");
        throw new Exception("Foo");
    }
}