using System.Threading.Tasks;
using ITJakub.MobileApps.Client.Core.ViewModel;

namespace ITJakub.MobileApps.Client.Core.Manager.Authentication
{
    public interface ILoginProvider
    {
        //public static ILoginProvider CreateLoginManager(LoginProviderType loginProviderType)
        //{
        //    switch (loginProviderType)
        //    {
        //        case LoginProviderType.LiveId:
        //            return new LiveIdProvider();
        //        case LoginProviderType.Facebook:
        //            return new FacebookProvider();
        //        case LoginProviderType.Google:
        //            return new GoogleProvider();
        //        default:
        //            return null;
        //    }
        //}

        Task<UserInfo> LoginAsync();

        string AccountName { get; }
        LoginProviderType ProviderType { get; }
    }
}
