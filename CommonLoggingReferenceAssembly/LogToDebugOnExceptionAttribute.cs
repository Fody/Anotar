using System;

namespace Anotar.CommonLogging
{
    /// <summary>
    /// If an <see cref="Exception"/> occurs in the applied method then log it to <c>Debug</c>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, Inherited = false)]
    public class LogToDebugOnExceptionAttribute : Attribute
    {
    }
}