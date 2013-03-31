using System;

namespace Anotar.Log4Net
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module, AllowMultiple = false, Inherited = false)]
    public class LogMinimalMessageAttribute : Attribute
    {
    }
}