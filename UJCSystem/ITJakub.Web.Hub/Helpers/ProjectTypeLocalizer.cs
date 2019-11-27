using System;
using System.Collections.Generic;
using System.Linq;
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

        public IList<ProjectTypeContract> GetProjectTypeList()
        {
            return Enum.GetValues(typeof(ProjectTypeContract)).Cast<ProjectTypeContract>().ToList();
        }

        public string TranslateBookFrom(ProjectTypeContract projectType)
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

        public string TranslatePortal(ProjectTypeContract projectType)
        {
            switch (projectType)
            {
                case ProjectTypeContract.Research:
                    return m_localizationService.Translate("ResearchPortal", "BibliographyModule");
                case ProjectTypeContract.Community:
                    return m_localizationService.Translate("CommunityPortal", "BibliographyModule");
                case ProjectTypeContract.Bibliography:
                    return m_localizationService.Translate("BibliographicRecords", "BibliographyModule");
                default:
                    return projectType.ToString();
            }
        }

        public string TranslatePortalShort(ProjectTypeContract projectType)
        {
            switch (projectType)
            {
                case ProjectTypeContract.Research:
                    return m_localizationService.Translate("ResearchPortalShort", "BibliographyModule");
                case ProjectTypeContract.Community:
                    return m_localizationService.Translate("CommunityPortalShort", "BibliographyModule");
                case ProjectTypeContract.Bibliography:
                    return m_localizationService.Translate("BibliographicRecordsShort", "BibliographyModule");
                default:
                    return projectType.ToString();
            }
        }
    }
}