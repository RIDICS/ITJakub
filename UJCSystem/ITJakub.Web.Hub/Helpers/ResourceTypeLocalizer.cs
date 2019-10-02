using System;
using Scalesoft.Localization.AspNetCore;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace ITJakub.Web.Hub.Helpers
{
    public class ResourceTypeLocalizer
    {
        private readonly ILocalizationService m_localizationService;

        public ResourceTypeLocalizer(ILocalizationService localizationService)
        {
            m_localizationService = localizationService;
        }

        public string TranslateResourceType(ResourceTypeEnumContract resourceType)
        {
            switch (resourceType)
            {
                case ResourceTypeEnumContract.Page:
                    return m_localizationService.Translate("Page", "RidicsProject");
                case ResourceTypeEnumContract.AudioTrack:
                    return m_localizationService.Translate("Track", "RidicsProject");
                case ResourceTypeEnumContract.Chapter:
                    return m_localizationService.Translate("Chapter", "RidicsProject");
                default:
                    return resourceType.ToString();
            }
        }

        public string TranslateRelatedResourceTypeFor(ResourceTypeEnumContract resourceType)
        {
            switch (resourceType)
            {
                case ResourceTypeEnumContract.Text:
                case ResourceTypeEnumContract.Image:
                    return TranslateResourceType(ResourceTypeEnumContract.Page);
                case ResourceTypeEnumContract.Audio:
                    return TranslateResourceType(ResourceTypeEnumContract.AudioTrack);
                default:
                    throw new ArgumentOutOfRangeException(nameof(resourceType), resourceType, null);
            }
        }
    }
}