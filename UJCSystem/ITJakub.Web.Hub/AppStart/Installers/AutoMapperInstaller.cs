using AutoMapper;
using ITJakub.Web.Hub.Areas.Admin.AutomapperProfiles;
using ITJakub.Web.Hub.AutoMapperProfiles;
using Vokabular.Shared.Container;

namespace ITJakub.Web.Hub.AppStart.Installers
{
    public class AutoMapperInstaller : IContainerInstaller
    {
        public void Install(IIocContainer container)
        {
            container.AddSingleton<Profile, ConditionCriteriaDescriptionProfile>();
            container.AddSingleton<Profile, DatingCriteriaDescriptionProfile>();
            container.AddSingleton<Profile, DatingListCriteriaDescriptionProfile>();
            container.AddSingleton<Profile, FavoriteProfile>();
            container.AddSingleton<Profile, StaticTextProfile>();
            container.AddSingleton<Profile, TokenDistanceCriteriaDescriptionProfile>();
            container.AddSingleton<Profile, TokenDistanceListCriteriaDescriptionProfile>();
            container.AddSingleton<Profile, WordCriteriaDescriptionProfile>();
            container.AddSingleton<Profile, WordListCriteriaDescriptionProfile>();

            // Admin area
            container.AddSingleton<Profile, LiteraryGenreProfile>();
            container.AddSingleton<Profile, LiteraryKindProfile>();
            container.AddSingleton<Profile, LiteraryOriginalProfile>();
            container.AddSingleton<Profile, ProjectProfile>();
            container.AddSingleton<Profile, ResourceProfile>();
            container.AddSingleton<Profile, ResponsibleTypeProfile>();
            container.AddSingleton<Profile, SnapshotProfile>();
            container.AddSingleton<Profile, UserProfile>();
        }
    }
}