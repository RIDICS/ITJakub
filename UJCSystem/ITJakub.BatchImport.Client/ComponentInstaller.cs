using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using ITJakub.BatchImport.Client.BusinessLogic;
using ITJakub.BatchImport.Client.BusinessLogic.Communication;
using Vokabular.MainService.DataContracts;
using Vokabular.MainService.DataContracts.Clients;

namespace ITJakub.BatchImport.Client
{
    public class ComponentInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {            
            container.Register(Component.For<FileUploadManager>());
            container.Register(Component.For<CommunicationProvider>());
            container.Register(Component.For<IMainServiceAuthTokenProvider, AuthenticationManager>());
            container.Register(Component.For<IMainServiceUriProvider, MainServiceUriProvider>());
            container.Register(Component.For<MainServiceRestClient>());
        }
    }
}