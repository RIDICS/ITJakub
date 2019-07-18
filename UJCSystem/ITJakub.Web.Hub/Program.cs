using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Vokabular.Log4Net;

namespace ITJakub.Web.Hub
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }
        
        public static IWebHost BuildWebHost(string[] args)
        {
            return new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .ConfigureAppConfiguration((builderContext, builder) =>
                {
                    var env = builderContext.HostingEnvironment;

                    var globalbuilder = new ConfigurationBuilder()
                        .SetBasePath(env.ContentRootPath)
                        .AddJsonFile("globalsettings.json");
                    var globalConfiguration = globalbuilder.Build();

                    var secretSettingsPath = globalConfiguration["SecretSettingsPath"];
                    var environmentConfiguration = globalConfiguration["EnvironmentConfiguration"];
                    var portalType = globalConfiguration["PortalType"];

                    builder
                        //.SetBasePath(env.ContentRootPath)
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{environmentConfiguration}.json", optional: true)
                        .AddJsonFile(Path.Combine(secretSettingsPath, "ITJakub.Secrets.json"), optional: true)
                        .AddJsonFile(Path.Combine(secretSettingsPath, $"ITJakub.Secrets.{environmentConfiguration}.json"), optional: true)
                        .AddJsonFile($"portalconfig.{portalType}.json", optional: false, reloadOnChange: true)
                        .AddEnvironmentVariables();
                })
                .ConfigureLogging((builderContext, builder) =>
                {
                    builderContext.HostingEnvironment.ConfigureLog4Net("log4net.config");
                    builder.AddConfiguration(builderContext.Configuration.GetSection("Logging"));
                    //builder.AddConsole();
                    //builder.AddDebug();
                    builder.AddLog4Net();
                })
                .UseStartup<Startup>()
                .Build();
        }
    }
}
