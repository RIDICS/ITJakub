using ITJakub.MobileApps.Client.Core.Manager.Authentication;
using ITJakub.MobileApps.Client.Core.Service;

namespace ITJakub.MobileApps.Client.Core.Manager.Converter
{
    public static class LoginProviderConverter
    {
        public static AuthenticationProviders LoginToAuthenticationProvider(LoginProviderType loginProvider)
        {
            AuthenticationProviders provider;
            switch (loginProvider)
            {
                case LoginProviderType.LiveId:
                    provider = AuthenticationProviders.LiveId;
                    break;
                case LoginProviderType.Google:
                    provider = AuthenticationProviders.Google;
                    break;
                case LoginProviderType.Facebook:
                    provider = AuthenticationProviders.Facebook;
                    break;
                default:
                    provider = AuthenticationProviders.ItJakub;
                    break;
            }
            return provider;
        }
    }
}