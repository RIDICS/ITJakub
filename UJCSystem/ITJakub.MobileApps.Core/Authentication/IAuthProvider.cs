using System;
using System.Collections.Generic;
using DotNetOpenAuth.FacebookOAuth2;
using DotNetOpenAuth.GoogleOAuth2;
using ITJakub.MobileApps.Core.Authentication.Image;
using ITJakub.MobileApps.DataContracts;
using ITJakub.MobileApps.DataEntities.Database.Repositories;
using User = ITJakub.MobileApps.DataEntities.Database.Entities.User;

namespace ITJakub.MobileApps.Core.Authentication
{
    public interface IAuthProvider
    {
        AuthenticationProviders ProviderType { get; }
        AuthenticateResultInfo Authenticate(string accessToken, string email);
    }


    public class GoogleAuthProvider : GoogleOAuth2Client, IAuthProvider
    {
        private const string PictureKey = "picture";
        private const string EmailKey = "email";

        public GoogleAuthProvider(string clientId = "none", string clientSecret = "none")
            : base(clientId, clientSecret)
        {
        }

        public AuthenticateResultInfo Authenticate(string accessToken, string email)
        {
            IDictionary<string, string> data = base.GetUserData(accessToken);
            bool authResult = data[EmailKey].Equals(email);
            var result = new AuthenticateResultInfo
            {
                Result = authResult ? AuthResultType.Success : AuthResultType.Failed,
            };
            if (!authResult)
                return result;

            result.UserImageLocation = GetImageLocation(data);

            return result;
        }

        public AuthenticationProviders ProviderType
        {
            get { return AuthenticationProviders.Google; }
        }

        private string GetImageLocation(IDictionary<string, string> userId)
        {
            if (userId.ContainsKey(PictureKey))
                return userId[PictureKey];
            return null;
        }
    }

    public class FacebookAuthProvider : FacebookOAuth2Client, IAuthProvider
    {
        private const string PictureKey = "picture";
        private const string EmailKey = "email";

        public FacebookAuthProvider(string clientId = "none", string clientSecret = "none")
            : base(clientId, clientSecret)
        {
        }


        public AuthenticateResultInfo Authenticate(string accessToken, string email)
        {
            IDictionary<string, string> data = base.GetUserData(accessToken);
            bool authResult = data[EmailKey].Equals(email);
            var result = new AuthenticateResultInfo
            {
                Result = authResult ? AuthResultType.Success : AuthResultType.Failed,
            };
            if (!authResult)
                return result;

            result.UserImageLocation = GetImageLocation(data);
            return result;
        }

        public AuthenticationProviders ProviderType
        {
            get { return AuthenticationProviders.Facebook; }
        }

        private string GetImageLocation(IDictionary<string, string> userId)
        {
            if (userId.ContainsKey(PictureKey))
                return userId[PictureKey];
            return null;
        }
    }

    public class ItJakubAuthProvider : IAuthProvider
    {
        private readonly UsersRepository m_usersRepository;
        private readonly GravatarImageUrlProvider m_imageUrlProvider;

        public ItJakubAuthProvider(UsersRepository usersRepository, GravatarImageUrlProvider imageUrlProvider)
        {
            m_usersRepository = usersRepository;
            m_imageUrlProvider = imageUrlProvider;
        }

        public AuthenticationProviders ProviderType
        {
            get { return AuthenticationProviders.ItJakub; }
        }

        public AuthenticateResultInfo Authenticate(string passwordHash, string email)
        {
            User user = m_usersRepository.FindByEmailAndProvider(email, (byte) AuthenticationProviders.ItJakub);
            bool authResult = user.PasswordHash.Equals(passwordHash);
            var result = new AuthenticateResultInfo
            {
                Result = authResult ? AuthResultType.Success : AuthResultType.Failed,
            };
            if (!authResult)
                return result;

            result.UserImageLocation = GetImageLocation(email);
            return result;
        }

        private string GetImageLocation(string email)
        {
            return m_imageUrlProvider.GetImageUrl(email);
        }
    }

    public class LiveIdAuthProvider : IAuthProvider
    {
        public AuthenticationProviders ProviderType
        {
            get { return AuthenticationProviders.LiveId; }
        }

        public AuthenticateResultInfo Authenticate(string accessToken, string email)
        {
            throw new NotImplementedException();
        }
    }
}