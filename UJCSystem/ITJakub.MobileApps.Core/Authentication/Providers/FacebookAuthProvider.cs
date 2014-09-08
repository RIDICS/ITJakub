using System.Collections.Generic;
using DotNetOpenAuth.FacebookOAuth2;
using ITJakub.MobileApps.DataContracts;

namespace ITJakub.MobileApps.Core.Authentication.Providers
{
    public class FacebookAuthProvider : FacebookOAuth2Client, IAuthProvider
    {
        private const string PictureKey = "picture";
        private const string EmailKey = "email";

        public FacebookAuthProvider(string clientId = "none", string clientSecret = "none")
            : base(clientId, clientSecret)
        {
        }

        public bool IsExternalProvider
        {
            get { return true; }
        }

        public AuthenticateResultInfo Authenticate(string accessToken, string email)
        {
            IDictionary<string, string> data = base.GetUserData(accessToken);
            bool authSucceeded = data[EmailKey].Equals(email);
            var result = new AuthenticateResultInfo
            {
                Result = authSucceeded ? AuthResultType.Success : AuthResultType.Failed,
            };
            if (authSucceeded)
                result.UserImageLocation = GetImageLocation(data);
            return result;
        }


        public AuthProvidersContract ProviderContractType
        {
            get { return AuthProvidersContract.Facebook; }
        }

        private string GetImageLocation(IDictionary<string, string> userId)
        {
            return userId.ContainsKey(PictureKey) ? userId[PictureKey] : null;
        }
    }
}