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
    sealed class StringFormatMethodAttribute : Attribute
    {
		/// <summary>
		/// Not for external use.
		/// </summary>
		// ReSharper disable once UnusedParameter.Local
        public StringFormatMethodAttribute(string formatParameterName)
        {
        }

    }
}