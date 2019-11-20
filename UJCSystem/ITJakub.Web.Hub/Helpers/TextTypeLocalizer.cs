using Scalesoft.Localization.AspNetCore;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace ITJakub.Web.Hub.Helpers
{
    public class TextTypeLocalizer
    {
        private readonly ILocalizationService m_localizationService;

        public TextTypeLocalizer(ILocalizationService localizationService)
        {
            m_localizationService = localizationService;
        }

        public string TranslateTextType(TextTypeEnumContract textType)
        {
            switch (textType)
            {
                case TextTypeEnumContract.Transliterated:
                    return m_localizationService.Translate("Transliterated", "Admin");
                case TextTypeEnumContract.Transcribed:
                    return m_localizationService.Translate("Transcribed", "Admin");
                default:
                    return textType.ToString();
            }
        }
    }
}