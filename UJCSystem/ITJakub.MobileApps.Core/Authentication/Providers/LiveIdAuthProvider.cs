using System;
using ITJakub.MobileApps.DataContracts;
using User = ITJakub.MobileApps.DataEntities.Database.Entities.User;

namespace ITJakub.MobileApps.Core.Authentication.Providers
{
    public class LiveIdAuthProvider : IAuthProvider
    {
        public AuthenticationProviders ProviderType
        {
            get { return AuthenticationProviders.LiveId; }
        }

        public bool IsExternalProvider
        {
            get { return true; }
        }

        private AuthenticateResultInfo Authenticate(string accessToken, string email)
        {
            throw new NotImplementedException();
        }

        public AuthenticateResultInfo Authenticate(UserLogin userLogin, User dbUser)
        {
            return Authenticate(userLogin.AuthenticationToken, dbUser.Email);
        }
    }
}