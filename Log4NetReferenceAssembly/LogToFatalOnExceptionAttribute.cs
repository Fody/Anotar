using System;

namespace Anotar.Log4Net
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    public class LogToFatalOnExceptionAttribute : Attribute
    {
    }
}