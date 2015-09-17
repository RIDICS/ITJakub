using System.Threading.Tasks;
using ITJakub.MobileApps.DataContracts;

namespace ITJakub.MobileApps.Client.Core.Manager.Authentication.AuthenticationProviders
{
    public interface ILoginProvider
    {
        Task<UserLoginSkeleton> LoginAsync();

        Task<UserLoginSkeleton> LoginForCreateUserAsync();

        string AccountName { get; }

        AuthProvidersContract ProviderType { get; }

        Task<UserLoginSkeleton> ReopenWithErrorAsync();
    }
}
