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
            container.AddPerWebRequest<ProjectMetadataManager>();
            container.AddPerWebRequest<UserManager>();

            container.AddSingleton<Profile, LiteraryGenreProfile>();
            container.AddSingleton<Profile, LiteraryKindProfile>();
            container.AddSingleton<Profile, ProjectProfile>();
            container.AddSingleton<Profile, PublisherProfile>();
            container.AddSingleton<Profile, UserProfile>();
        }
    }
}
