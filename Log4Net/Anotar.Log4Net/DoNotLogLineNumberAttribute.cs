using System;

namespace Anotar.Log4Net
{
    /// <summary>
    /// Used to suppress message prefixing.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module)]
    public class DoNotLogLineNumberAttribute : Attribute
    {
    }
}