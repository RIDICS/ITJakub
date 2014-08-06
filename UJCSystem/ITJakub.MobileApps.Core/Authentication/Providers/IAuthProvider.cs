using ITJakub.MobileApps.DataContracts;
using User = ITJakub.MobileApps.DataEntities.Database.Entities.User;

namespace ITJakub.MobileApps.Core.Authentication.Providers
{
    public interface IAuthProvider
    {
        AuthenticationProviders ProviderType { get; }
        bool IsExternalProvider { get; }
        AuthenticateResultInfo Authenticate(UserLogin userLogin, User dbUser);
    }
}