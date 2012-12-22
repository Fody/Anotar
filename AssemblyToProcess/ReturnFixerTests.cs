using System;
using System.Diagnostics;
using Anotar;

public class ReturnFixerTests
{

    [LogToDebugOnException]
    public object MethodWithHangingHandlerEnd()
    {
        try
        {
            throw new Exception("Foo");
        }
        finally
        {
            Debug.WriteLine("finally");
        }

    }


    [LogToDebugOnException]
    public void MethodWithHangingHandlerEnd2()
    {
        try
        {
            if (DateTime.Now == DateTime.Now)
                throw new Exception("Foo");
        }
        finally
        {
            Debug.WriteLine("finally");
        }

        Debug.WriteLine("finally");
    }

// ReSharper disable NotAccessedField.Local
    int x;
// ReSharper restore NotAccessedField.Local
    public bool HasValue;

    [LogToDebugOnException]
    public void BranchingReturn()
    {
        if (HasValue)
        {
            x++;
        }
        else
        {
            x++;

        }
    }
}