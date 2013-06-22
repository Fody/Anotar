using System;

namespace Anotar.Custom
{
    /// <summary>
    /// Used to point to the correct logger factory Template.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class LoggerFactoryAttribute : Attribute
    {

        /// <summary>
        /// Construct a new instance of <see cref="LoggerFactoryAttribute"/>
        /// </summary>
        /// <param name="loggerFactory">The logger factory <see cref="Type"/> to use.</param>
        public LoggerFactoryAttribute(Type loggerFactory)
        {

        }
    }
}