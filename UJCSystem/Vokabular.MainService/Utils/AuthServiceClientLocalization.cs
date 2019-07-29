using Ridics.Authentication.HttpClient.Client;
using Ridics.Authentication.HttpClient.Exceptions;

namespace Vokabular.MainService.Utils
{
    public class AuthServiceClientLocalization : IAuthorizationServiceClientLocalization
    {
        public void LocalizeApiException(AuthServiceApiException ex)
        {
            // no localization support on Main Service
        }

        public string GetCurrentCulture()
        {
            // no localization support on Main Service, return default language
            return "cs-CZ";
        }
    }
}
