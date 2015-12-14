using System;

namespace Anotar.Serilog
{
	/// <summary>
	/// Provides logging functions.
	/// </summary>
    public static class LogTo
    {
        

        /// <summary>
        /// Returns true if verbose is enabled.
        /// </summary>
        public static bool IsVerboseEnabled
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Writes the diagnostic message at the <c>Verbose</c> level.
        /// </summary>
        public static void Verbose()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes the diagnostic message at the <c>Verbose</c> level.
        /// </summary>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        public static void Verbose(string messageTemplate, params object[] propertyValues)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes the diagnostic message and exception at the <c>Verbose</c> level.
        /// </summary>
        /// <param name="exception">An exception to be logged.</param>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        public static void Verbose(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Returns true if debug is enabled.
        /// </summary>
        public static bool IsDebugEnabled
        {
            get { throw new NotImplementedException(); }
        }

		/// <summary>
		/// Writes the diagnostic message at the <c>Debug</c> level.
		/// </summary>
        public static void Debug()
        {
            throw new NotImplementedException();
        }

		/// <summary>
		/// Writes the diagnostic message at the <c>Debug</c> level.
		/// </summary>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        public static void Debug(string messageTemplate, params object[] propertyValues)
        {
            throw new NotImplementedException();
        }

	    /// <summary>
	    /// Writes the diagnostic message and exception at the <c>Debug</c> level.
	    /// </summary>
        /// <param name="exception">An exception to be logged.</param>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        public static void Debug(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            throw new NotImplementedException();
        }

		/// <summary>
		/// Writes the diagnostic message at the <c>Info</c> level.
		/// </summary>
        public static void Information()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns true if information is enabled.
        /// </summary>
        public static bool IsInformationEnabled
        {
            get { throw new NotImplementedException(); }
        }

		/// <summary>
		/// Writes the diagnostic message at the <c>Info</c> level.
        /// </summary>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        public static void Information(string messageTemplate, params object[] propertyValues)
        {
            throw new NotImplementedException();
        }

	    /// <summary>
	    /// Writes the diagnostic message and exception at the <c>Info</c> level.
	    /// </summary>
        /// <param name="exception">An exception to be logged.</param>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        public static void Information(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns true if warning is enabled.
        /// </summary>
        public static bool IsWarningEnabled
        {
            get { throw new NotImplementedException(); }
        }

		/// <summary>
        /// Writes the diagnostic message at the <c>Warn</c> level.
		/// </summary>
        public static void Warning()
        {
            throw new NotImplementedException();
        }

		/// <summary>
		/// Writes the diagnostic message at the <c>Warn</c> level.
        /// </summary>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        public static void Warning(string messageTemplate, params object[] propertyValues)
        {
            throw new NotImplementedException();
        }

	    /// <summary>
	    /// Writes the diagnostic message and exception at the <c>Warn</c> level.
	    /// </summary>
        /// <param name="exception">An exception to be logged.</param>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        public static void Warning(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns true if error is enabled.
        /// </summary>
        public static bool IsErrorEnabled
        {
            get { throw new NotImplementedException(); }
        }

		/// <summary>
        /// Writes the diagnostic message at the <c>Error</c> level.
		/// </summary>
        public static void Error()
        {
            throw new NotImplementedException();
        }

		/// <summary>
        /// Writes the diagnostic message at the Error level.
        /// </summary>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        public static void Error(string messageTemplate, params object[] propertyValues)
        {
            throw new NotImplementedException();
        }

	    /// <summary>
	    /// Writes the diagnostic message and exception at the <c>Error</c> level.
	    /// </summary>
        /// <param name="exception">An exception to be logged.</param>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        public static void Error(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns true if fatal is enabled.
        /// </summary>
        public static bool IsFatalEnabled
        {
            get { throw new NotImplementedException(); }
        }

		/// <summary>
		/// Writes the diagnostic message at the <c>Fatal</c> level.
		/// </summary>
        public static void Fatal()
        {
            throw new NotImplementedException();
        }

		/// <summary>
		/// Writes the diagnostic message at the <c>Fatal</c> level.
        /// </summary>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        public static void Fatal(string messageTemplate, params object[] propertyValues)
        {
            throw new NotImplementedException();
        }

	    /// <summary>
	    /// Writes the diagnostic message and exception at the <c>Fatal</c> level.
	    /// </summary>
        /// <param name="exception">An exception to be logged.</param>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        public static void Fatal(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            throw new NotImplementedException();
        }
    }
}

// ReSharper restore UnusedParameter.Global
