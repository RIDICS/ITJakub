﻿using System.Net;
using System.ServiceModel.Web;
using ITJakub.MobileApps.DataContracts;
using ITJakub.MobileApps.DataEntities.Database.Repositories;

namespace ITJakub.MobileApps.Core.Authentication
{
    public class AuthenticationManager
    {
        private readonly AuthProviderDirector m_authDirector;
        private readonly CommunicationTokenManager m_communicationTokenManager;
        private readonly UsersRepository m_usersRepository;

        public AuthenticationManager(AuthProviderDirector authDirector, UsersRepository usersRepository, CommunicationTokenManager communicationTokenManager)
        {
            m_authDirector = authDirector;
            m_usersRepository = usersRepository;
            m_communicationTokenManager = communicationTokenManager;
        }


        public void AuthenticateByCommunicationToken(string communicationToken, Role minRoleAllowed = Role.Student, long? requiredUserId = null)
        {
            var user = m_usersRepository.GetUserByCommunicationToken(communicationToken);
            if (user == null || !m_communicationTokenManager.IsCommunicationTokenActive(user.CommunicationTokenCreateTime))
                throw new WebFaultException(HttpStatusCode.Unauthorized) {Source = "Recieved token expired or is not valid. Login again please..."};
            if (minRoleAllowed.Equals(Role.Teacher) && user.Institution == null)
                throw new WebFaultException(HttpStatusCode.Unauthorized) {Source = "You don't have enough privileges ..."};
            if (requiredUserId!=null && !user.Id.Equals(requiredUserId))
                throw new WebFaultException(HttpStatusCode.Unauthorized) {Source = "Recieved token does not belong to recieved user identificator ..."};
        }

        public void AuthenticateByProvider(string email, string authenticationToken, AuthenticationProviders authenticationProvider)
        {
            if (!m_authDirector.GetProvider(authenticationProvider).Authenticate(authenticationToken, email))
                throw new WebFaultException(HttpStatusCode.Unauthorized) {Source = "Users e-mail is not valid."};
        }
    }
}