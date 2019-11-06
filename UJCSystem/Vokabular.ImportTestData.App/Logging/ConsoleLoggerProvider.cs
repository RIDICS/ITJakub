using Microsoft.Extensions.Logging;

namespace Vokabular.ImportTestData.App.Logging
{
    public class ConsoleLoggerProvider : ILoggerProvider
    {
        public void Dispose()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new ConsoleLogger(categoryName);
        }
    }
}