using AutoMapper;
using Vokabular.Core;
using Vokabular.MainService.Core.AutoMapperProfiles;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.Core.Managers;
using Vokabular.Shared.Container;

namespace Vokabular.MainService.Core
{
    public class MainServiceCoreContainerRegistration : IContainerInstaller
    {
        public void Install(IIocContainer container)
        {
            container.AddPerWebRequest<BookManager>();
            container.AddPerWebRequest<CategoryManager>();
            container.AddPerWebRequest<PageManager>();
            container.AddPerWebRequest<PersonManager>();
            container.AddPerWebRequest<ProjectManager>();
            container.AddPerWebRequest<ProjectMetadataManager>();
            container.AddPerWebRequest<ProjectResourceManager>();
            container.AddPerWebRequest<UserManager>();

            container.AddPerWebRequest<CommunicationConfigurationProvider>();
            container.AddPerWebRequest<CommunicationProvider>();

            container.AddSingleton<Profile, BookProfile>();
            container.AddSingleton<Profile, CategoryProfile>();
            container.AddSingleton<Profile, KeywordProfile>();
            container.AddSingleton<Profile, LiteraryGenreProfile>();
            container.AddSingleton<Profile, LiteraryKindProfile>();
            container.AddSingleton<Profile, MetadataProfile>();
            container.AddSingleton<Profile, OriginalAuthorProfile>();
            container.AddSingleton<Profile, PageProfile>();
            container.AddSingleton<Profile, ProjectProfile>();
            container.AddSingleton<Profile, ResponsiblePersonProfile>();
            container.AddSingleton<Profile, TextProfile>();
            container.AddSingleton<Profile, UserProfile>();

            container.Install<CoreContainerRegistration>();
        }
    }
}
