using System;

namespace Anotar.Splat
{
    /// <summary>
    /// Used to suppress message prefixing.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module)]
    public class LogMinimalMessageAttribute : Attribute
    {
    }
}