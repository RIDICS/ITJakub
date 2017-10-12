using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Vokabular.Log4Net
{
    public class Log4NetProvider : ILoggerProvider
    {
        private readonly Assembly m_repositoryAssembly;

        private IDictionary<string, ILogger> m_loggers
            = new Dictionary<string, ILogger>();

        public Log4NetProvider(Assembly repositoryAssembly)
        {
            m_repositoryAssembly = repositoryAssembly;
        }

        public ILogger CreateLogger(string name)
        {
            if (!m_loggers.ContainsKey(name))
            {
                lock (m_loggers)
                {
                    // Have to check again since another thread may have gotten the lock first
                    if (!m_loggers.ContainsKey(name))
                    {
                        m_loggers[name] = new Log4NetAdapter(m_repositoryAssembly, name);
                    }
                }
            }
            return m_loggers[name];
        }

        public void Dispose()
        {
            m_loggers.Clear();
            m_loggers = null;
        }
    }
}
