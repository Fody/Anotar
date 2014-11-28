using System;

namespace Anotar.CommonLogging
{
    /// <summary>
    /// If an <see cref="Exception"/> occurs in the applied method then log it to <c>Warn</c>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, Inherited = false)]
    public class LogToWarnOnExceptionAttribute : Attribute
    {
    }
}