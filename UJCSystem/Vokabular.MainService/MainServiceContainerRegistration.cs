using Microsoft.Extensions.DependencyInjection;
using Vokabular.DataEntities;
using Vokabular.MainService.Core;
using Vokabular.Shared.Container;

namespace Vokabular.MainService
{
    public class MainServiceContainerRegistration : IContainerInstaller
    {
        public void Install(IServiceCollection services)
        {
            new MainServiceCoreContainerRegistration().Install(services);
            new DataEntitiesContainerRegistration().Install(services);
        }
    }
}