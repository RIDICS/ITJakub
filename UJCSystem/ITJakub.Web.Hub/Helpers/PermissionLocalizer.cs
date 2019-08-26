using Microsoft.Extensions.Logging;
using Scalesoft.Localization.AspNetCore;
using Scalesoft.Localization.Core.Exception;
using Vokabular.Shared;

namespace ITJakub.Web.Hub.Helpers
{
    public class PermissionLocalizer
    {
        private readonly ILocalizationService m_localizationService;
        private static readonly ILogger m_logger = ApplicationLogging.CreateLogger<PermissionLocalizer>();

        public PermissionLocalizer(ILocalizationService localizationService)
        {
            m_localizationService = localizationService;
        }

        public string TranslatePermissionName(string key)
        {
            try
            {
                var localizedString = m_localizationService.Translate(key, "PermissionNames");

                if (localizedString.ResourceNotFound)
                {
                    throw new TranslateException(); // Force fallback. Explicitly throw exception, when localization library is configured to not throw exception on key not found
                }

                return localizedString.Value;
            }
            catch (TranslateException)
            {
                if (m_logger.IsEnabled(LogLevel.Warning))
                {
                    m_logger.LogWarning($"Translation key: {key} in scope: PermissionNames not found");
                }

                return key;
            }
        }
    }
}
