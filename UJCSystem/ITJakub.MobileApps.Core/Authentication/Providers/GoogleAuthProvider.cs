using System.Collections.Generic;
using DotNetOpenAuth.GoogleOAuth2;
using ITJakub.MobileApps.DataContracts;
using User = ITJakub.MobileApps.DataEntities.Database.Entities.User;

namespace ITJakub.MobileApps.Core.Authentication.Providers
{
    public class GoogleAuthProvider : GoogleOAuth2Client, IAuthProvider
    {
        private const string PictureKey = "picture";
        private const string EmailKey = "email";

        public GoogleAuthProvider(string clientId = "none", string clientSecret = "none")
            : base(clientId, clientSecret)
        {
        }

        public bool IsExternalProvider
        {
            get { return true; }
        }

        private AuthenticateResultInfo Authenticate(string accessToken, string email)
        {
            IDictionary<string, string> userData = base.GetUserData(accessToken);
            bool authSucceeded = userData[EmailKey].Equals(email);
            var result = new AuthenticateResultInfo
            {
                Result = authSucceeded ? AuthResultType.Success : AuthResultType.Failed,
            };
            if (authSucceeded)
                result.UserImageLocation = GetImageLocation(userData);
            return result;
        }

        public AuthenticateResultInfo Authenticate(UserLogin userLogin, User dbUser)
        {
            return Authenticate(userLogin.AuthenticationToken, dbUser.Email);
        }

        public AuthenticationProviders ProviderType
        {
            get { return AuthenticationProviders.Google; }
        }

        private string GetImageLocation(IDictionary<string, string> userData)
        {
            return userData.ContainsKey(PictureKey) ? userData[PictureKey] : null;
        }
    }
}