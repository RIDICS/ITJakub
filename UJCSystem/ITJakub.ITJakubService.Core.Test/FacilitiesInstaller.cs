using Castle.Facilities.AutoTx;
using Castle.Facilities.NHibernate;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace ITJakub.ITJakubService.Core.Test
{
    public class FacilitiesInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container
                .AddFacility<AutoTxFacility>()
                .AddFacility<TypedFactoryFacility>()
                .Register(Component.For<IConfigurationPersister>().ImplementedBy<FileConfigurationPersister>(),
                    Component.For<INHibernateInstaller>().ImplementedBy<NHibernateInstaller>().LifeStyle.Singleton)
                .AddFacility<NHibernateFacility>();
        }
    }
}