using System.Linq;
using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Managers.Authentication;
using Vokabular.MainService.Core.Works.Users;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Errors;
using Vokabular.RestClient.Headers;

namespace Vokabular.MainService.Core.Managers
{
    public class AuthenticationManager
    {
        private readonly UserRepository m_userRepository;
        private readonly ICommunicationTokenGenerator m_communicationTokenGenerator;
        private readonly IHttpContextAccessor m_httpContextAccessor;

        public AuthenticationManager(UserRepository userRepository, ICommunicationTokenGenerator communicationTokenGenerator, IHttpContextAccessor httpContextAccessor)
        {
            m_userRepository = userRepository;
            m_communicationTokenGenerator = communicationTokenGenerator;
            m_httpContextAccessor = httpContextAccessor;
        }

        public User GetCurrentUser()
        {
            if (!m_httpContextAccessor.HttpContext.Request.Headers.TryGetValue(CustomHttpHeaders.Authorization, out var communicationTokens))
            {
                throw new HttpErrorCodeException("User not signed in", HttpStatusCode.Unauthorized);
            }

            var communicationToken = communicationTokens.First();
            var user = m_userRepository.InvokeUnitOfWork(x => x.GetUserByToken(communicationToken));
            return user;
        }

        public int GetCurrentUserId()
        {
            return GetCurrentUser().Id;
        }
        
        public SignInResultContract RenewCommunicationToken()
        {
            var userId = GetCurrentUserId();
            var work = new RenewCommunicationTokenWork(m_userRepository, m_communicationTokenGenerator, userId);
            work.Execute();

            return new SignInResultContract
            {
                CommunicationToken = work.CommunicationToken
            };
        }

        public SignInResultContract SignIn(SignInContract data)
        {
            var work = new SignInWork(m_userRepository, m_communicationTokenGenerator, data);
            work.Execute();

            return new SignInResultContract
            {
                CommunicationToken = work.CommunicationToken
            };
        }

        public void SignOut(string authorizationToken)
        {
            new SignOutWork(m_userRepository, authorizationToken).Execute();
        }
    }

    public class UserManager
    {
        private readonly UserRepository m_userRepository;
        private readonly ICommunicationTokenGenerator m_communicationTokenGenerator;

        public UserManager(UserRepository userRepository, ICommunicationTokenGenerator communicationTokenGenerator)
        {
            m_userRepository = userRepository;
            m_communicationTokenGenerator = communicationTokenGenerator;
        }

        public int CreateNewUser(CreateUserContract data)
        {
            // TODO add data validation (min lenght, e-mail is valid, etc.)

            var userId = new CreateNewUserWork(m_userRepository, m_communicationTokenGenerator, data).Execute();
            return userId;
        }

        public UserDetailContract GetUserByToken(string authorizationToken)
        {
            var dbUser = m_userRepository.InvokeUnitOfWork(x => x.GetUserByToken(authorizationToken));
            var result = Mapper.Map<UserDetailContract>(dbUser);
            return result;
        }

        public void UpdateUser(string authorizationToken, UpdateUserContract data)
        {
            // TODO add data validation

            new UpdateUserWork(m_userRepository, authorizationToken, data).Execute();
        }

        public void UpdateUserPassword(string authorizationToken, UpdateUserPasswordContract data)
        {
            // TODO add data validation

            new UpdateUserPasswordWork(m_userRepository, authorizationToken, data).Execute();
        }
    }
}
