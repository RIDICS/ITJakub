using Log4net.Extensions.Logging;
using Microsoft.Extensions.Logging;

namespace ITJakub.Web.Hub.AppStart.Loggers
{
    public class CustomLog4NetProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new Log4NetAdapter(categoryName);
        }

        public void Dispose()
        {
        }
    }
}
