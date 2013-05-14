
// ReSharper disable UnusedParameter.Global
using System;

namespace Anotar.Serilog
{
	/// <summary>
	/// Provides logging functions.
	/// </summary>
    public static class LogTo
    {
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
        /// Writes the diagnostic message at the <c>Error</c> level.
		/// </summary>
        public static void Error()
        {
            throw new NotImplementedException();
        }

		/// <summary>
        /// Writes the diagnostic message at the <see cref="Serilog.e.LogEventLevel."/>  level.
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
