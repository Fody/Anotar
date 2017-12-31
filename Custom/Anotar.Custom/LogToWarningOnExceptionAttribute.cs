using System;

namespace Anotar.Custom
{
    /// <summary>
    /// If an <see cref="Exception"/> occurs in the applied method then log it to <c>Warning</c>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, Inherited = false)]
    public class LogToWarningOnExceptionAttribute : Attribute
    {
    }
}