using System;

namespace Anotar.NLog
{
    /// <summary>
    /// Used to suppress message prefixing.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module, AllowMultiple = false, Inherited = false)]
    public class LogMinimalMessageAttribute : Attribute
    {
    }
}