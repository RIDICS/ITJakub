using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Vokabular.Log4Net
{
    public static class Log4NetAspExtensions
    {
        public static void ConfigureLog4Net(this IHostingEnvironment appEnv, string configFileRelativePath)
        {
            ConfigureLog4Net(appEnv.ContentRootPath, configFileRelativePath);
        }

        public static void ConfigureLog4Net(string currentDir, string configFileRelativePath)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            GlobalContext.Properties["appRoot"] = currentDir;
            XmlConfigurator.Configure(logRepository, new FileInfo(Path.Combine(currentDir, configFileRelativePath)));
        }

        public static void AddLog4Net(this ILoggerFactory loggerFactory)
        {
            loggerFactory.AddProvider(new Log4NetProvider(Assembly.GetEntryAssembly()));
        }

        public static ILoggingBuilder AddLog4Net(this ILoggingBuilder loggingBuilder)
        {
            loggingBuilder.AddProvider(new Log4NetProvider(Assembly.GetEntryAssembly()));

            return loggingBuilder;
        }
    }
}