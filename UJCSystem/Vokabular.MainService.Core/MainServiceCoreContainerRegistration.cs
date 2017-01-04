using AutoMapper;
using Vokabular.MainService.Core.AutoMapperProfiles;
using Vokabular.MainService.Core.Managers;
using Vokabular.Shared.Container;

namespace Vokabular.MainService.Core
{
    public class MainServiceCoreContainerRegistration : IContainerInstaller
    {
        public void Install(IContainer container)
        {
            container.AddPerWebRequest<ProjectManager>();
            container.AddPerWebRequest<UserManager>();

            container.AddSingleton<Profile, ProjectProfile>();
            container.AddSingleton<Profile, UserProfile>();
        }
    }
}
