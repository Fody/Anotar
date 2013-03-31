using System;

namespace Anotar.Serilog
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    public class LogToInfoOnExceptionAttribute : Attribute
    {
    }
}