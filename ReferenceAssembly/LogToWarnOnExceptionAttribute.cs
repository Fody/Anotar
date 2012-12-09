using System;

namespace Anotar
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    public class LogToWarnOnExceptionAttribute : Attribute
    {
    }
}