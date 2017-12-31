using System;

namespace Anotar.CommonLogging
{
    /// <summary>
    /// Used to suppress message prefixing.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module)]
    public class LogMinimalMessageAttribute : Attribute
    {
    }
}