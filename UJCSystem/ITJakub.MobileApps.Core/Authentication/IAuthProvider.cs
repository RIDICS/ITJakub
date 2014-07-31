using System;
using DotNetOpenAuth.FacebookOAuth2;
using DotNetOpenAuth.GoogleOAuth2;
using ITJakub.MobileApps.DataContracts;
using ITJakub.MobileApps.DataEntities.Database.Repositories;

namespace ITJakub.MobileApps.Core.Authentication
{
    public interface IAuthProvider
    {
        string GetEmail(string accessToken);
        AuthenticationProviders ProviderType { get; }
    }


    public class GoogleAuthProvider : GoogleOAuth2Client, IAuthProvider
    {
        public GoogleAuthProvider(string clientId = "none", string clientSecret = "none")
            : base(clientId, clientSecret)
        {
        }

        public string GetEmail(string accessToken)
        {
            var data = base.GetUserData(accessToken);
            return data["email"];
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


        public string GetEmail(string accessToken)
        {
            var data = base.GetUserData(accessToken);
            return data["email"];
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

        public string GetEmail(string accessToken)
        {
            var user = m_usersRepository.FindByAuthenticationProviderToken(accessToken);
            return user.Email;
        }

        public AuthenticationProviders ProviderType
        {
            get { return AuthenticationProviders.ItJakub; }
        }
    }

    public class LiveIdAuthProvider : IAuthProvider
    {
        public string GetEmail(string accessToken)
        {
            throw new NotImplementedException();
        }

        public AuthenticationProviders ProviderType
        {
            get { return AuthenticationProviders.LiveId; }
        }
    }
}