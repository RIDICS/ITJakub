using Vokabular.MainService.Core.Managers;
using Vokabular.Shared.Container;

namespace Vokabular.MainService.Core
{
    public class MainServiceCoreContainerRegistration : IContainerInstaller
    {
        public void Install(IContainer container)
        {
            container.AddPerWebRequest<ProjectManager>();
            //container.AddPerWebRequest<CreateProjectWork>();
        }
    }
}
