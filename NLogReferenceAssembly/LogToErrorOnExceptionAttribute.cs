using System;

namespace Anotar.NLog
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    public class LogToErrorOnExceptionAttribute : Attribute
    {
    }
}