using System;
using System.Reflection;
using log4net;
using Microsoft.Extensions.Logging;

namespace Vokabular.Log4Net
{
    public class Log4NetAdapter : ILogger
    {
        private readonly ILog m_logger;

        public Log4NetAdapter(Assembly repositoryAssembly, string loggerName)
        {
            m_logger = LogManager.GetLogger(repositoryAssembly, loggerName);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                    return m_logger.IsDebugEnabled;
                case LogLevel.Information:
                    return m_logger.IsInfoEnabled;
                case LogLevel.Warning:
                    return m_logger.IsWarnEnabled;
                case LogLevel.Error:
                    return m_logger.IsErrorEnabled;
                case LogLevel.Critical:
                    return m_logger.IsFatalEnabled;
                case LogLevel.None:
                    return false;
                default:
                    throw new ArgumentException($"Unknown log level {logLevel}.", nameof(logLevel));
            }
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }
            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            switch (logLevel)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                    m_logger.Debug(message, exception);
                    break;
                case LogLevel.Information:
                    m_logger.Info(message, exception);
                    break;
                case LogLevel.Warning:
                    m_logger.Warn(message, exception);
                    break;
                case LogLevel.Error:
                    m_logger.Error(message, exception);
                    break;
                case LogLevel.Critical:
                    m_logger.Fatal(message, exception);
                    break;
                case LogLevel.None:
                    break;
                default:
                    m_logger.Warn($"Encountered unknown log level {logLevel}, writing out as Info.");
                    m_logger.Info(message, exception);
                    break;
            }
        }
    }
}