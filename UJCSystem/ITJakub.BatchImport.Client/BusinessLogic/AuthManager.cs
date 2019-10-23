using System.Configuration;
using System.Threading.Tasks;
using Vokabular.AppAuthentication.Shared;

namespace ITJakub.BatchImport.Client.BusinessLogic
{
    public class AuthManager
    {
        private readonly AuthenticationManager m_authenticationManager;
        private const string OidcUrl = "OIDCUrl";
        private const string OidcClientId = "OIDCClientId";
        private const string OidcClientSecret = "OIDCClientSecret";

        public AuthManager(AuthenticationManager authenticationManager)
        {
            m_authenticationManager = authenticationManager;
        }

        public Task SignInAsync()
        {
            return m_authenticationManager.SignInAsync(new AuthenticationOptions
            {
                Url = ConfigurationManager.AppSettings[OidcUrl],
                ClientId = ConfigurationManager.AppSettings[OidcClientId],
                ClientSecret = ConfigurationManager.AppSettings[OidcClientSecret],
            });
        }
    }
}
