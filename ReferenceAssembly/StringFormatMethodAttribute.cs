using System;

namespace JetBrains.Annotations
{ 
    [Obsolete("Not for external use.")]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, AllowMultiple = false, Inherited = true)]
    public sealed class StringFormatMethodAttribute : Attribute
    {
// ReSharper disable UnusedParameter.Local
        public StringFormatMethodAttribute(string formatParameterName)
// ReSharper restore UnusedParameter.Local
        {
        }

    }
}