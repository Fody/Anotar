using System;

namespace Anotar.NServiceBus
{
    /// <summary>
    /// If an <see cref="Exception"/> occurs in the applied method then log it to <c>Info</c>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, Inherited = false)]
    public class LogToInfoOnExceptionAttribute : Attribute
    {
    }
}