using Microsoft.Extensions.DependencyInjection;
using Vokabular.DataEntities;
using Vokabular.MainService.Core;
using Vokabular.Shared.Container;

namespace Vokabular.MainService
{
    public class MainServiceContainerRegistration : IContainerInstaller
    {
        public void Install(IIocContainer container)
        {
            new MainServiceCoreContainerRegistration().Install(container);
            var services = new ServiceCollection();
            services.AddDataEntitiesServices();
            container.Populate(services);
        }
    }
}