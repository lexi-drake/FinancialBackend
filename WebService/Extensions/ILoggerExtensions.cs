using System;
using Serilog;

namespace WebService
{
    public static class ILoggerExtensions
    {
        public static void Throw(this ILogger logger, string message)
        {
            logger.Error(message);
            throw new ArgumentException(message);
        }
    }
}