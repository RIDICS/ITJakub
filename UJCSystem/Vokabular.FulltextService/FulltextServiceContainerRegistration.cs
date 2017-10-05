using Vokabular.FulltextService.Core;
using Vokabular.Shared.Container;

namespace Vokabular.FulltextService
{
    public class FulltextServiceContainerRegistration : IContainerInstaller
    {
        public void Install(IIocContainer container)
        {
            container.Install<FulltextServiceCoreContainerRegistration>();
        }
    }
}