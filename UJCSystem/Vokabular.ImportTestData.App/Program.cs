using System;
using System.IO;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Vokabular.AppAuthentication.Shared;
using Vokabular.MainService.DataContracts;
using Vokabular.MainService.DataContracts.Contracts.Type;

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

            var app = container.Resolve<ImportTestDataApp>();
            app.Run();
        }
    }
}
