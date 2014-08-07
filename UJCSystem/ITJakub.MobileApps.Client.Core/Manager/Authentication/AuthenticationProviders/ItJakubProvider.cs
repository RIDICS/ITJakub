using System.Threading.Tasks;
using ITJakub.MobileApps.Client.Core.ViewModel;

namespace ITJakub.MobileApps.Client.Core.Manager.Authentication.AuthenticationProviders
{
    public class ItJakubProvider : ILoginProvider
    {
        public Task<UserLoginSkeleton> LoginAsync()
        {
            throw new System.NotImplementedException();
        }

        public string AccountName { get { return "It Jakub"; } }
        public LoginProviderType ProviderType { get{return LoginProviderType.ItJakub;} }
    }
}