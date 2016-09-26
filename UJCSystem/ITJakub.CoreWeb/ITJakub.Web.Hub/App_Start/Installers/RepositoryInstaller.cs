using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using ITJakub.Web.DataEntities.Database.Repositories;

namespace ITJakub.Web.Hub.App_Start.Installers
{
    public class RepositoryInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<StaticTextRepository>());
        }
    }
}
