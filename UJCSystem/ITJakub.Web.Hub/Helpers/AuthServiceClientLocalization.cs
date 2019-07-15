using Ridics.Authentication.HttpClient.Client;
using Ridics.Authentication.HttpClient.Exceptions;
using Scalesoft.Localization.AspNetCore;

namespace ITJakub.Web.Hub.Helpers
{
    public class AuthServiceClientLocalization : IAuthorizationServiceClientLocalization
    {
        private readonly ILocalizationService m_localizationService;

        public AuthServiceClientLocalization(ILocalizationService localizationService)
        {
            m_localizationService = localizationService;
        }

        public void LocalizeApiException(AuthServiceApiException ex)
        {
            // Web Hub doesn't use AuthService client directly
        }

        public string GetCurrentCulture()
        {
            return m_localizationService.GetRequestCulture().Name;
        }
    }
}
