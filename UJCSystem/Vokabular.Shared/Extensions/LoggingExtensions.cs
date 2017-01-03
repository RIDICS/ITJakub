﻿using Microsoft.Extensions.Logging;

namespace Vokabular.Shared.Extensions
{
    public static class LoggingExtensions
    {
        public static bool IsCriticalEnabled(this ILogger logger)
        {
            return logger.IsEnabled(LogLevel.Critical);
        }

        public static bool IsDebugEnabled(this ILogger logger)
        {
            return logger.IsEnabled(LogLevel.Debug);
        }

        public static bool IsErrorEnabled(this ILogger logger)
        {
            return logger.IsEnabled(LogLevel.Error);
        }

        public static bool IsInformationEnabled(this ILogger logger)
        {
            return logger.IsEnabled(LogLevel.Information);
        }

        public static bool IsTraceEnabled(this ILogger logger)
        {
            return logger.IsEnabled(LogLevel.Trace);
        }

        public static bool IsWarningEnabled(this ILogger logger)
        {
            return logger.IsEnabled(LogLevel.Warning);
        }
    }
}
