using Microsoft.Extensions.DependencyInjection;
using Vokabular.FulltextService.Core;
using Vokabular.Shared.Container;

namespace Vokabular.FulltextService
{
    public class FulltextServiceContainerRegistration : IContainerInstaller
    {
        public void Install(IServiceCollection services)
        {
            new FulltextServiceCoreContainerRegistration().Install(services);
        }
    }
}