using System;
using System.Collections.Generic;
using DotNetOpenAuth.FacebookOAuth2;
using DotNetOpenAuth.GoogleOAuth2;
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
        private readonly string m_imageUrlStringFormat;

        public GoogleAuthProvider(string clientId = "none", string clientSecret = "none", string imageUrlStringFormat = null)
            : base(clientId, clientSecret)
        {
            m_imageUrlStringFormat = imageUrlStringFormat;
        }

        public AuthenticateResultInfo Authenticate(string accessToken, string email)
        {
            IDictionary<string, string> data = base.GetUserData(accessToken);
            bool authResult = data["email"].Equals(email);
            var result = new AuthenticateResultInfo
            {
                Result = authResult ? AuthResultType.Success : AuthResultType.Failed,
            };
            if (!authResult)
                return result;

            result.UserImageLocation = GetImageLocation(data["userId"]);

            return result;
        }

        public AuthenticationProviders ProviderType
        {
            get { return AuthenticationProviders.Google; }
        }

        private string GetImageLocation(string userId)
        {
            return m_imageUrlStringFormat != null ? string.Format(m_imageUrlStringFormat, userId) : null;
        }
    }

    public class FacebookAuthProvider : FacebookOAuth2Client, IAuthProvider
    {
        private readonly string m_imageUrlStringFormat;

        public FacebookAuthProvider(string clientId = "none", string clientSecret = "none", string imageUrlStringFormat = null)
            : base(clientId, clientSecret)
        {
            m_imageUrlStringFormat = imageUrlStringFormat;
        }


        public AuthenticateResultInfo Authenticate(string accessToken, string email)
        {
            IDictionary<string, string> data = base.GetUserData(accessToken);
            bool authResult = data["email"].Equals(email);
            var result = new AuthenticateResultInfo
            {
                Result = authResult ? AuthResultType.Success : AuthResultType.Failed,
            };
            if (!authResult)
                return result;

            result.UserImageLocation = GetImageLocation(data["userId"]);
            return result;
        }

        public AuthenticationProviders ProviderType
        {
            get { return AuthenticationProviders.Facebook; }
        }

        private string GetImageLocation(string userId)
        {
            return m_imageUrlStringFormat != null ? string.Format(m_imageUrlStringFormat, userId) : null;
        }
    }

    public class ItJakubAuthProvider : IAuthProvider
    {
        private readonly UsersRepository m_usersRepository;

        public ItJakubAuthProvider(UsersRepository usersRepository)
        {
            m_usersRepository = usersRepository;
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
            return null;
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