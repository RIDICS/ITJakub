using System;
using DotNetOpenAuth.FacebookOAuth2;
using DotNetOpenAuth.GoogleOAuth2;
using ITJakub.MobileApps.DataContracts;
using ITJakub.MobileApps.DataEntities.Database.Repositories;

namespace ITJakub.MobileApps.Core.Authentication
{
    public interface IAuthProvider
    {
        bool Authenticate(string accessToken, string email);
        AuthenticationProviders ProviderType { get; }
    }


    public class GoogleAuthProvider : GoogleOAuth2Client, IAuthProvider
    {
        public GoogleAuthProvider(string clientId = "none", string clientSecret = "none")
            : base(clientId, clientSecret)
        {
        }

        public bool Authenticate(string accessToken, string email)
        {
            var data = base.GetUserData(accessToken);
            return data["email"].Equals(email);
        }

        public AuthenticationProviders ProviderType
        {
            get { return AuthenticationProviders.Google; }
        }
    }

    public class FacebookAuthProvider : FacebookOAuth2Client, IAuthProvider
    {
        public FacebookAuthProvider(string clientId = "none", string clientSecret = "none")
            : base(clientId, clientSecret)
        {
        }


        public bool Authenticate(string accessToken, string email)
        {
            var data = base.GetUserData(accessToken);
            return data["email"].Equals(email);
        }

        public AuthenticationProviders ProviderType
        {
            get { return AuthenticationProviders.Facebook; }
        }
    }

    public class ItJakubAuthProvider : IAuthProvider
    {
        private readonly UsersRepository m_usersRepository;

        public ItJakubAuthProvider(UsersRepository usersRepository)
        {
            m_usersRepository = usersRepository;
        }

        public bool Authenticate(string passwordHash, string email)
        {
            var user = m_usersRepository.FindByEmailAndProvider(email,(byte) AuthenticationProviders.ItJakub);
            return user.PasswordHash.Equals(passwordHash);
        }

        public AuthenticationProviders ProviderType
        {
            get { return AuthenticationProviders.ItJakub; }
        }
    }

    public class LiveIdAuthProvider : IAuthProvider
    {
        public bool Authenticate(string accessToken, string email)
        {
            throw new NotImplementedException();
        }

        public AuthenticationProviders ProviderType
        {
            get { return AuthenticationProviders.LiveId; }
        }
    }
}