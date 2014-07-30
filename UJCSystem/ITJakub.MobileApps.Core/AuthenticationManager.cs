using System;
using Castle.MicroKernel;
using DotNetOpenAuth.GoogleOAuth2;
using ITJakub.MobileApps.DataContracts;
using ITJakub.MobileApps.DataEntities.Database.Repositories;

namespace ITJakub.MobileApps.Core
{
    public class AuthenticationManager
    {
        private readonly UsersRepository m_usersRepository;
        private readonly TimeSpan m_timeToTokenExpiration;

        public AuthenticationManager(UsersRepository usersRepository, int timeToTokenExpiration)
        {
            m_usersRepository = usersRepository;
            m_timeToTokenExpiration = new TimeSpan(0, 0, timeToTokenExpiration);
        }


        public void AuthenticateByCommunicationToken(string communicationToken)
        {
            if (!m_usersRepository.IsCommunicationTokenValid(communicationToken, m_timeToTokenExpiration)) throw new Exception("Recieved token expired or is not valid. Login again please..."); //TODO throw better exception
        }

        public void AuthenticateByProvider(string email, string accessToken, AuthenticationProviders authenticationProvider)
        {
            //IAuthProvider client = new GoogleClient("***REMOVED***", "***REMOVED***");
            IAuthProvider client = new Google Client("none", "none");
            var a=client.GetEmail(accessToken);
        }



    }

    public class GoogleClient : GoogleOAuth2Client, IAuthProvider 
    {
        public GoogleClient(string clientId, string clientSecret) : base(clientId, clientSecret)
        {
        }

        public GoogleClient(string clientId, string clientSecret, params string[] requestedScopes) : base(clientId, clientSecret, requestedScopes)
        {
        }

        public string GetEmail(string accessToken)
        {
            var data = base.GetUserData(accessToken);
            return data["email"];
        }
    }

    public interface IAuthProvider
    {
        string GetEmail(string accessToken);
    }
}