using System;
using Scalpel;

[Remove]
public class LogEvent
{
    public object[] Args;
    public string Format;
    public Exception Exception;
}