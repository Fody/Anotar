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
    public void WithLdflda(decimal value)
    {
        if (value == 0.0m)
        {
            property1 = null;
        }
        else
        {
            property1 = value;
        }
    }
    
    [LogToDebugOnException]
    public void WithTernary(decimal? value)
    {
        var newValue = value == 0.0m ? null : value;
        if (property1 != newValue)
        {
            property1 = newValue;
        }
    }

    decimal? property1;
    [LogToDebugOnException]
    public void WithLdfldaShortCircut(decimal value)
    {
        if (value == 0.0m)
        {
            if (property1 == null)
            {
                return;
            }
            property1 = null;
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

    bool isInSomeMode;
    string branchingReturnField;

    [LogToDebugOnException]
    public void BranchingReturn(string value)
    {
        branchingReturnField = value;
        if (isInSomeMode)
        {
            Console.WriteLine("code here so 'if' does not get optimized away in release mode");
            return;
        }
    }
}