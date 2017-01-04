using AutoMapper;
using ITJakub.Web.Hub.Areas.Admin.AutomapperProfiles;
using ITJakub.Web.Hub.AutoMapperProfiles;
using Vokabular.Shared.Container;

namespace ITJakub.Web.Hub.Installers
{
    public class AutoMapperInstaller : IContainerInstaller
    {
        public void Install(IContainer container)
        {
            container.AddPerWebRequest<Profile, ConditionCriteriaDescriptionProfile>();
            container.AddPerWebRequest<Profile, DatingCriteriaDescriptionProfile>();
            container.AddPerWebRequest<Profile, DatingListCriteriaDescriptionProfile>();
            container.AddPerWebRequest<Profile, FavoriteProfile>();
            container.AddPerWebRequest<Profile, StaticTextProfile>();
            container.AddPerWebRequest<Profile, TokenDistanceCriteriaDescriptionProfile>();
            container.AddPerWebRequest<Profile, TokenDistanceListCriteriaDescriptionProfile>();
            container.AddPerWebRequest<Profile, WordCriteriaDescriptionProfile>();
            container.AddPerWebRequest<Profile, WordListCriteriaDescriptionProfile>();

            // Admin area
            container.AddPerWebRequest<Profile, ProjectProfile>();
            container.AddPerWebRequest<Profile, ResourceProfile>();
            container.AddPerWebRequest<Profile, SnapshotProfile>();
            container.AddPerWebRequest<Profile, UserProfile>();
        }
    }
}