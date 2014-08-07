using System.Threading.Tasks;
using ITJakub.MobileApps.Client.Core.ViewModel;

namespace ITJakub.MobileApps.Client.Core.Manager.Authentication.AuthenticationProviders
{
    public interface ILoginProvider
    {
        Task<UserLoginSkeleton> LoginAsync();

        string AccountName { get; }
        LoginProviderType ProviderType { get; }
    }
}
