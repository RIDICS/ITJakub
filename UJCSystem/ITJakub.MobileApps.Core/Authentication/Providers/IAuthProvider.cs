using ITJakub.MobileApps.DataContracts;
using User = ITJakub.MobileApps.DataEntities.Database.Entities.User;

namespace ITJakub.MobileApps.Core.Authentication.Providers
{
    public interface IAuthProvider
    {
        AuthProvidersContract ProviderContractType { get; }
        bool IsExternalProvider { get; }
        AuthenticateResultInfo Authenticate(string providerToken, string email);
    }
}