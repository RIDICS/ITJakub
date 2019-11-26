using Scalesoft.Localization.AspNetCore;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace ITJakub.Web.Hub.Helpers
{
    public class ProjectTypeLocalizer
    {
        private readonly ILocalizationService m_localizationService;

        public ProjectTypeLocalizer(ILocalizationService localizationService)
        {
            m_localizationService = localizationService;
        }

        public string TranslateProjectType(ProjectTypeContract projectType)
        {
            switch (projectType)
            {
                case ProjectTypeContract.Research:
                    return m_localizationService.Translate("BookFromResearchPortal", "BibliographyModule");
                case ProjectTypeContract.Community:
                    return m_localizationService.Translate("BookFromCommunityPortal", "BibliographyModule");
                case ProjectTypeContract.Bibliography:
                    return m_localizationService.Translate("BibliographicRecord", "BibliographyModule");
                default:
                    return projectType.ToString();
            }
        }
    }
}