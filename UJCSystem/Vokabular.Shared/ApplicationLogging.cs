using Microsoft.Extensions.Logging;

namespace Vokabular.Shared
{
    public static class ApplicationLogging
    {
        public static ILoggerFactory LoggerFactory { get; set; }

        public static ILogger CreateLogger<T>()
        {
            return LoggerFactory.CreateLogger<T>();
        }
    }
}
