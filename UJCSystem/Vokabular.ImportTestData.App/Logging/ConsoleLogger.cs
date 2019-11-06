using System;
using Microsoft.Extensions.Logging;

namespace Vokabular.ImportTestData.App.Logging
{
    public class ConsoleLogger : ILogger
    {
        private readonly string m_categoryName;

        public ConsoleLogger(string categoryName)
        {
            m_categoryName = categoryName;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var message = formatter(state, exception);

            Console.Error.WriteLine($"[{logLevel}] {m_categoryName} {message}");
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotSupportedException();
        }
    }
}