using System;

namespace Anotar.NServiceBus
{
    /// <summary>
    /// Used to suppress message prefixing.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module)]
    public class DoNotLogLineNumberAttribute : Attribute
    {
    }
}