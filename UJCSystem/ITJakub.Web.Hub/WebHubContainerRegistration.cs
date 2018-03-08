using AutoMapper;
using ITJakub.Web.Hub.Areas.Admin.AutomapperProfiles;
using ITJakub.Web.Hub.AutoMapperProfiles;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Core.Managers;
using ITJakub.Web.Hub.Core.Markdown;
using Vokabular.Shared.Container;

namespace ITJakub.Web.Hub
{
    public class WebHubContainerRegistration : IContainerInstaller
    {
        public void Install(IIocContainer container)
        {
            container.AddPerWebRequest<CommunicationProvider>();
            container.AddPerWebRequest<CommunicationConfigurationProvider>();
            container.AddPerWebRequest<StaticTextManager>();
            container.AddPerWebRequest<FeedbacksManager>();
            container.AddPerWebRequest<AuthenticationManager>();

            container.AddPerWebRequest<IMarkdownToHtmlConverter, MarkdigMarkdownToHtmlConverter>();

            // AutoMapper profiles
            container.AddSingleton<Profile, ConditionCriteriaDescriptionProfile>();
            container.AddSingleton<Profile, DatingCriteriaDescriptionProfile>();
            container.AddSingleton<Profile, DatingListCriteriaDescriptionProfile>();
            container.AddSingleton<Profile, FavoriteProfile>();
            container.AddSingleton<Profile, TokenDistanceCriteriaDescriptionProfile>();
            container.AddSingleton<Profile, TokenDistanceListCriteriaDescriptionProfile>();
            container.AddSingleton<Profile, WordCriteriaDescriptionProfile>();
            container.AddSingleton<Profile, WordListCriteriaDescriptionProfile>();

            // AutoMapper profiles - Admin area
            container.AddSingleton<Profile, LiteraryGenreProfile>();
            container.AddSingleton<Profile, LiteraryKindProfile>();
            container.AddSingleton<Profile, LiteraryOriginalProfile>();
            container.AddSingleton<Profile, CategoryProfile>();
            container.AddSingleton<Profile, ProjectProfile>();
            container.AddSingleton<Profile, ResourceProfile>();
            container.AddSingleton<Profile, ResponsibleTypeProfile>();
            container.AddSingleton<Profile, SnapshotProfile>();
            container.AddSingleton<Profile, UserProfile>();
        }
    }
}
