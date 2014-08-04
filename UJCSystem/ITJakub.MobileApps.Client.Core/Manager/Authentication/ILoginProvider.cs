using System.Threading.Tasks;
using ITJakub.MobileApps.Client.Core.ViewModel;

namespace ITJakub.MobileApps.Client.Core.Manager.Authentication
{
    public interface ILoginProvider
    {
        Task<UserInfo> LoginAsync();

        string AccountName { get; }
        LoginProviderType ProviderType { get; }
    }
}
