using System;

namespace Anotar.Custom
{
    /// <summary>
    /// Used to point to the correct logger factory Template.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class LoggerFactoryAttribute : Attribute
    {

        /// <summary>
        /// Construct a new instance of <see cref="LoggerFactoryAttribute"/>
        /// </summary>
        /// <param name="loggerFactory">The logger factory <see cref="Type"/> to use.</param>
        // ReSharper disable once UnusedParameter.Local
        public LoggerFactoryAttribute(Type loggerFactory)
        {

        }
    }
}