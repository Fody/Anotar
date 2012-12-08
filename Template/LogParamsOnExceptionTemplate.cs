using System;
using Anotar;
using NLog;

namespace Before
{
    public class SimpleClass
    {

        [LogToTraceOnException]
        void Method1(string param1, int param2)
        {
            //Do Stuff
        }
        [LogToDebugOnException]
        void Method2(string param1, int param2)
        {
            //Do Stuff
        }
    }
}

namespace After
{

    public class SimpleClass
    {
        static Logger logger;

        static SimpleClass()
        {
            logger = LogManager.GetCurrentClassLogger();
        }

        void Method1(string param1, int param2)
        {
            try
            {
                //Do Stuff
            }
            catch (Exception exception)
            {
                if (logger.IsDebugEnabled)
                {
                    var message = string.Format("Exception occurred in SimpleClass.Method1. param1 '{0}', param2 '{1}'", new object[] { param1, param2 });
                    logger.DebugException(message, exception);
                }
                throw;
            }
        }
        int Method1WithReturn(string param1, int param2, ref int param3)
        {
            try
            {
                return param2;
            }
            catch (Exception exception)
            {
                var message = string.Format("Exception occurred in SimpleClass.Method1. param1 '{0}', param2 '{1}'", new object[] {  param3 });
                if (logger.IsDebugEnabled)
                {
                    logger.DebugException(message, exception);
                }
                if (logger.IsErrorEnabled)
                {
                    logger.Error(message, exception);
                }
                throw;
            }
        }
        void Method2(string param1, int param2)
        {
            try
            {
                //Do Stuff
            }
            catch (Exception exception)
            {
                if (logger.IsTraceEnabled)
                {
                    var message = string.Format("Exception occurred in SimpleClass.Method2. param1 '{0}', param2 '{1}'", param1, param2);
                    logger.TraceException(message, exception);
                }
                throw;
            }
        }
    }
}