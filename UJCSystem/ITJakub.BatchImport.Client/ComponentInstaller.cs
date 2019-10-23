using System;
using System.Configuration;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using ITJakub.BatchImport.Client.BusinessLogic;
using ITJakub.BatchImport.Client.BusinessLogic.Communication;
using Microsoft.Extensions.DependencyInjection;
using Vokabular.AppAuthentication.Shared;
using Vokabular.MainService.DataContracts;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.Shared.WcfService;

namespace ITJakub.BatchImport.Client
{
    public class ComponentInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var mainServiceConfiguration = new MainServiceClientConfiguration
            {
                PortalType = PortalTypeContract.Research,
                Url = new Uri(ConfigurationManager.AppSettings["MainService"])
            };

            container.Register(Component.For<AuthManager>());
            container.Register(Component.For<FileUploadManager>());
            container.Register(Component.For<CommunicationProvider>());

            var services = new ServiceCollection();
            services.RegisterAppAuthenticationServices(mainServiceConfiguration);
            
            container.AddServicesCollection(services);
        }
    }
}