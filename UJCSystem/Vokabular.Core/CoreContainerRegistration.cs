using Vokabular.Core.Search;
using Vokabular.Shared.Container;

namespace Vokabular.Core
{
    public class CoreContainerRegistration : IContainerInstaller
    {
        public void Install(IIocContainer container)
        {
            container.AddPerWebRequest<MetadataSearchCriteriaDirector>();
            container.AddPerWebRequest<MetadataSearchCriteriaProcessor>();
        }
    }
}
