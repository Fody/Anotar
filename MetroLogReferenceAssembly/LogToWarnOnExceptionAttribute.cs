using System;

namespace Anotar.MetroLog
{
    /// <summary>
    /// If an <see cref="Exception"/> occurs in the applied method then log it to <c>Warn</c>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    public class LogToWarnOnExceptionAttribute : Attribute
    {
    }
}