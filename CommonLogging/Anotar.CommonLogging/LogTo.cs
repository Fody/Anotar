﻿using JetBrains.Annotations;

namespace Anotar.CommonLogging;

///<summary>
///Provides logging functions.
///</summary>
public static class LogTo
{
    /// <summary>
    /// Returns true if trace is enabled.
    /// </summary>
    public static bool IsTraceEnabled => throw new NotImplementedException();

    /// <summary>
    /// Writes the diagnostic message at the <c>Trace</c> level.
    /// </summary>
    public static void Trace()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Writes the diagnostic message at the <c>Trace</c> level.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public static void Trace(string message)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Writes the diagnostic message at the <c>Trace</c> level.
    /// </summary>
    /// <param name="format">A <see langword="string" /> containing format items.</param>
    /// <param name="args">Arguments to format.</param>
    [StringFormatMethod("format")]
    public static void Trace(string format, params object[] args)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Writes the diagnostic message at the <c>Trace</c> level.
    /// </summary>
    /// <param name="message">The log message.</param>
    public static void Trace(Func<string> message)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Writes the diagnostic message and exception at the <c>Trace</c> level.
    /// </summary>
    /// <param name="format">A <see langword="string" /> containing format items.</param>
    /// <param name="exception">An exception to be logged.</param>
    /// <param name="args">Arguments to format.</param>
    [StringFormatMethod("format")]
    public static void TraceException(string format, Exception exception, params object[] args)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Writes the diagnostic message and exception at the <c>Trace</c> level.
    /// </summary>
    /// <param name="message">The log message.</param>
    /// <param name="exception">An exception to be logged.</param>
    public static void TraceException(Func<string> message, Exception exception)
    {
        throw new NotImplementedException();
    }

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
    public static void DebugException(string format, Exception exception, params object[] args)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Writes the diagnostic message and exception at the <c>Debug</c> level.
    /// </summary>
    /// <param name="message">The log message.</param>
    /// <param name="exception">An exception to be logged.</param>
    public static void DebugException(Func<string> message, Exception exception)
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
    public static void InfoException(string format, Exception exception, params object[] args)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Writes the diagnostic message and exception at the <c>Info</c> level.
    /// </summary>
    /// <param name="message">The log message.</param>
    /// <param name="exception">An exception to be logged.</param>
    public static void InfoException(Func<string> message, Exception exception)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns true if warn is enabled.
    /// </summary>
    public static bool IsWarnEnabled => throw new NotImplementedException();

    /// <summary>
    /// Writes the diagnostic message at the <c>Warn</c> level.
    /// </summary>
    public static void Warn()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Writes the diagnostic message at the <c>Warn</c> level.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public static void Warn(string message)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Writes the diagnostic message at the <c>Warn</c> level.
    /// </summary>
    /// <param name="format">A <see langword="string" /> containing format items.</param>
    /// <param name="args">Arguments to format.</param>
    [StringFormatMethod("format")]
    public static void Warn(string format, params object[] args)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Writes the diagnostic message at the <c>Warn</c> level.
    /// </summary>
    /// <param name="message">The log message.</param>
    public static void Warn(Func<string> message)
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
    public static void WarnException(string format, Exception exception, params object[] args)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Writes the diagnostic message and exception at the <c>Warn</c> level.
    /// </summary>
    /// <param name="message">The log message.</param>
    /// <param name="exception">An exception to be logged.</param>
    public static void WarnException(Func<string> message, Exception exception)
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
    [StringFormatMethod("format")]
    public static void ErrorException(string format, Exception exception, params object[] args)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Writes the diagnostic message and exception at the <c>Error</c> level.
    /// </summary>
    /// <param name="message">The log message.</param>
    /// <param name="exception">An exception to be logged.</param>
    public static void ErrorException(Func<string> message, Exception exception)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns true if trace is enabled.
    /// </summary>
    public static bool IsFatalEnabled => throw new NotImplementedException();

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
    /// <param name="message">The message to log.</param>
    public static void Fatal(string message)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Writes the diagnostic message at the <c>Fatal</c> level.
    /// </summary>
    /// <param name="format">A <see langword="string" /> containing format items.</param>
    /// <param name="args">Arguments to format.</param>
    [StringFormatMethod("format")]
    public static void Fatal(string format, params object[] args)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Writes the diagnostic message at the <c>Fatal</c> level.
    /// </summary>
    /// <param name="message">The log message.</param>
    public static void Fatal(Func<string> message)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Writes the diagnostic message and exception at the <c>Fatal</c> level.
    /// </summary>
    /// <param name="format">A <see langword="string" /> containing format items.</param>
    /// <param name="exception">An exception to be logged.</param>
    /// <param name="args">Arguments to format.</param>
    [StringFormatMethod("format")]
    public static void FatalException(string format, Exception exception, params object[] args)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Writes the diagnostic message and exception at the <c>Fatal</c> level.
    /// </summary>
    /// <param name="message">The log message.</param>
    /// <param name="exception">An exception to be logged.</param>
    public static void FatalException(Func<string> message, Exception exception)
    {
        throw new NotImplementedException();
    }
}