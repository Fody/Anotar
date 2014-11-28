using System;

namespace Anotar.Serilog
{
    /// <summary>
    /// If an <see cref="Exception"/> occurs in the applied method then log it to <c>Information</c>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, Inherited = false)]
    public class LogToInformationOnExceptionAttribute : Attribute
    {
    }
}