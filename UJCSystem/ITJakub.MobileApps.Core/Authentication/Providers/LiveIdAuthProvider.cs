using System;
using System.Threading.Tasks;
using ITJakub.MobileApps.DataContracts;
using Microsoft.Live;
using User = ITJakub.MobileApps.DataEntities.Database.Entities.User;

namespace ITJakub.MobileApps.Core.Authentication.Providers
{
    public class LiveIdAuthProvider : IAuthProvider
    {
        public AuthProvidersContract ProviderContractType
        {
            get { return AuthProvidersContract.LiveId; }
        }

        public bool IsExternalProvider
        {
            get { return true; }
        }

        public AuthenticateResultInfo Authenticate(string accessToken, string email)
        {
            throw new NotImplementedException("Live Id not possible at this moment");
        }

        
    }
}