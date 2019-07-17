using System;

namespace Anotar.Serilog
{
    /// <summary>
    /// Used to suppress message prefixing.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module)]
    public class DoNotLogMethodNameAttribute : Attribute
    {
    }
}