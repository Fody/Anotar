using System;
using JetBrains.Annotations;

namespace Anotar.Catel
{
    ///<summary>
    ///Provides logging functions.
    ///</summary>
    public static class LogTo
    {
        /// <summary>
        /// Returns true if debug is enabled.
        /// </summary>
        public static bool IsDebugEnabled => throw new NotImplementedException();

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
        /// <param name="message">The message to log.</param>
        public static void Debug(string message)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes the diagnostic message at the <c>Debug</c> level.
        /// </summary>
        /// <param name="format">A <see langword="string" /> containing format items.</param>
        /// <param name="args">Arguments to format.</param>
        [StringFormatMethod("format")]
        public static void Debug(string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes the diagnostic message at the <c>Debug</c> level.
        /// </summary>
        /// <param name="message">The log message.</param>
        public static void Debug(Func<string> message)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes the diagnostic message and exception at the <c>Debug</c> level.
        /// </summary>
        /// <param name="format">A <see langword="string" /> containing format items.</param>
        /// <param name="exception">An exception to be logged.</param>
        /// <param name="args">Arguments to format.</param>
        [StringFormatMethod("format")]
        public static void Debug(Exception exception, string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes the diagnostic message and exception at the <c>Debug</c> level.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="exception">An exception to be logged.</param>
        public static void Debug(Exception exception, Func<string> message)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns true if info is enabled.
        /// </summary>
        public static bool IsInfoEnabled => throw new NotImplementedException();

        /// <summary>
        /// Writes the diagnostic message at the <c>Info</c> level.
        /// </summary>
        public static void Info()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes the diagnostic message at the <c>Info</c> level.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void Info(string message)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes the diagnostic message at the <c>Info</c> level.
        /// </summary>
        /// <param name="format">A <see langword="string" /> containing format items.</param>
        /// <param name="args">Arguments to format.</param>
        [StringFormatMethod("format")]
        public static void Info(string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes the diagnostic message at the <c>Info</c> level.
        /// </summary>
        /// <param name="message">The log message.</param>
        public static void Info(Func<string> message)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes the diagnostic message and exception at the <c>Info</c> level.
        /// </summary>
        /// <param name="format">A <see langword="string" /> containing format items.</param>
        /// <param name="exception">An exception to be logged.</param>
        /// <param name="args">Arguments to format.</param>
        [StringFormatMethod("format")]
        public static void Info(Exception exception, string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes the diagnostic message and exception at the <c>Info</c> level.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="exception">An exception to be logged.</param>
        public static void Info(Exception exception, Func<string> message)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns true if Warning is enabled.
        /// </summary>
        public static bool IsWarningEnabled => throw new NotImplementedException();

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
        /// <param name="message">The message to log.</param>
        public static void Warning(string message)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes the diagnostic message at the <c>Warn</c> level.
        /// </summary>
        /// <param name="format">A <see langword="string" /> containing format items.</param>
        /// <param name="args">Arguments to format.</param>
        [StringFormatMethod("format")]
        public static void Warning(string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes the diagnostic message at the <c>Warn</c> level.
        /// </summary>
        /// <param name="message">The log message.</param>
        public static void Warning(Func<string> message)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes the diagnostic message and exception at the <c>Warn</c> level.
        /// </summary>
        /// <param name="format">A <see langword="string" /> containing format items.</param>
        /// <param name="exception">An exception to be logged.</param>
        /// <param name="args">Arguments to format.</param>
        [StringFormatMethod("format")]
        public static void Warning(Exception exception, string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes the diagnostic message and exception at the <c>Warn</c> level.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="exception">An exception to be logged.</param>
        public static void Warning(Exception exception, Func<string> message)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns true if error is enabled.
        /// </summary>
        public static bool IsErrorEnabled => throw new NotImplementedException();

        /// <summary>
        /// Writes the diagnostic message at the <c>Error</c> level.
        /// </summary>
        public static void Error()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes the diagnostic message at the <c>Error</c> level.
        /// </summary>
        /// <param name="message">The message to log.</param>
        [StringFormatMethod("format")]
        public static void Error(string message)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes the diagnostic message at the <c>Error</c> level.
        /// </summary>
        /// <param name="format">A <see langword="string" /> containing format items.</param>
        /// <param name="args">Arguments to format.</param>
        [StringFormatMethod("format")]
        public static void Error(string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes the diagnostic message at the <c>Error</c> level.
        /// </summary>
        /// <param name="message">The log message.</param>
        public static void Error(Func<string> message)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes the diagnostic message and exception at the <c>Error</c> level.
        /// </summary>
        /// <param name="format">A <see langword="string" /> containing format items.</param>
        /// <param name="exception">An exception to be logged.</param>
        /// <param name="args">Arguments to format.</param>
        public static void Error(Exception exception, string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes the diagnostic message and exception at the <c>Error</c> level.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="exception">An exception to be logged.</param>
        public static void Error(Exception exception, Func<string> message)
        {
            throw new NotImplementedException();
        }
    }
}
