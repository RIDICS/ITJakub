using System;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vokabular.AppAuthentication.Shared;
using Vokabular.ImportTestData.App.Logging;
using Vokabular.MainService.DataContracts;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.Shared;

namespace Vokabular.ImportTestData.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var mainServiceConfiguration = new MainServiceClientConfiguration
            {
                PortalType = PortalTypeContract.Community,
                Url = new Uri("https://localhost:44331/api/"),
            };
            var authenticationOptions = new AuthenticationOptions
            {
                Url = "https://localhost:44395",
                ClientId = "VokabularBatchImport",
                ClientSecret = "secret",
            };

            var container = new Container();
            var services = new ServiceCollection();

            services.RegisterAppAuthenticationServices(mainServiceConfiguration);
            services.AddSingleton<ImportTestDataApp>();
            services.AddSingleton<ImportTestProjectManager>();
            services.AddSingleton(authenticationOptions);
            services.AddSingleton(new DataProvider(Console.Out, Console.In));

            container.Populate(services);

            ApplicationLogging.LoggerFactory = new LoggerFactory(new[] {new ConsoleLoggerProvider()});

            var app = container.Resolve<ImportTestDataApp>();
            app.Run();
        }
    }
}
