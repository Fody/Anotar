using System;

namespace Anotar.LibLog
{
    /// <summary>
    /// Used to suppress message prefixing.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module)]
    public class DoNotLogLineNumberAttribute : Attribute
    {
    }
}