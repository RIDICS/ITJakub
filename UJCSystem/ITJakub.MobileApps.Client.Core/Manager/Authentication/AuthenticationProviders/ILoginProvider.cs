using System.Threading.Tasks;

namespace ITJakub.MobileApps.Client.Core.Manager.Authentication.AuthenticationProviders
{
    public interface ILoginProvider
    {
        Task<UserLoginSkeleton> LoginAsync();

        string AccountName { get; }
        LoginProviderType ProviderType { get; }
    }
}
