## This is an add-in for  [Fody](https://github.com/SimonCropp/Fody) 

Change string comparisons to be case insensitive.

[Introduction to Fody](https://github.com/SimonCropp/Fody/wiki/SampleUsage)

## Nuget

Nuget package http://nuget.org/packages/Anotar.Fody 

## Given this as a reference

    public static class Log
    {
        public static void Debug()
        public static void Debug(string format)
        public static void Debug(string format, params object[] args)
        public static void Debug(string message, Exception exception)
        public static void Info(string message)
        public static void Info()
        public static void Info(string format, params object[] args)
        public static void Info(string message, Exception exception)
        public static void Warn(string message)
        public static void Warn()
        public static void Warn(string format, params object[] args)
        public static void Warn(string message, Exception exception)
        public static void Error()
        public static void Error(string message)
        public static void Error(string format, params object[] args)
        public static void Error(string message, Exception exception)
    }

## Your Code

    public class MyClass
    {
        void MyMethod()
        {
            Log.Debug("TheMessage");
        }
    }

## What gets compiled

### If you reference NLog

    public class MyClass
    {
        static NLog.Logger logger = NLog.LogManager.GetLogger("MyClass");

        void MyMethod()
        {
            logger.Debug("Method: MyMethod. Line: ~12. TheMessage");
        }
    }

### If you reference log4net

    public class MyClass
    {
        static log4net.ILog logger = log4net.LogManager.GetLogger("MyClass");

        void MyMethod()
        {
            logger.Debug("Method: MyMethod. Line: ~12. TheMessage");
        }
    }
