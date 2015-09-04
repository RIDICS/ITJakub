using System.Threading.Tasks;
using ITJakub.MobileApps.DataContracts;

namespace ITJakub.MobileApps.Client.Core.Manager.Authentication.AuthenticationProviders
{
    public class ItJakubProvider : ILoginProvider
    {
        public Task<UserLoginSkeleton> LoginAsync()
        {
            LocalAuthenticationBroker.LocalAuthenticationBroker.CreateUserAsync();
            return Task.Run(() => new UserLoginSkeleton
            {
                Success = false
            });

            // TODO ITJ login
        }

        public string AccountName { get { return "It Jakub"; } }
        public AuthProvidersContract ProviderType { get { return AuthProvidersContract.ItJakub; } }
    }
}