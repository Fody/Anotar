using System;

namespace Anotar.MetroLog
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    public class LogToFatalOnExceptionAttribute : Attribute
    {
    }
}