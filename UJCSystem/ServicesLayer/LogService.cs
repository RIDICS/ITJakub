using NLog;

namespace ServicesLayer
{
    public class LogService : ILogService
    {
        private static readonly Logger m_logger = LogManager.GetCurrentClassLogger();

        /// <see cref="ILogService" />
        public void LogDebug(string msg)
        {
            m_logger.Debug(msg);
        }

        /// <see cref="ILogService" />
        public void LogError(string msg)
        {
            m_logger.Error(msg);
        }

        /// <see cref="ILogService" />
        public void LogInfo(string msg)
        {
            m_logger.Info(msg);
        }

        /// <see cref="ILogService" />
        public void LogWarn(string msg)
        {
            m_logger.Warn(msg);
        }
    }
}
