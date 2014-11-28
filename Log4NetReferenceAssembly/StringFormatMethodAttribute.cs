using System;

namespace JetBrains.Annotations
{ 
#if (RELEASE)
    [Obsolete("Not for external use.")]
#endif
	/// <summary>
	/// Not for external use.
	/// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor)]
    internal sealed class StringFormatMethodAttribute : Attribute
    {
		/// <summary>
		/// Not for external use.
		/// </summary>
// ReSharper disable UnusedParameter.Local
        public StringFormatMethodAttribute(string formatParameterName)
// ReSharper restore UnusedParameter.Local
        {
        }

    }
}