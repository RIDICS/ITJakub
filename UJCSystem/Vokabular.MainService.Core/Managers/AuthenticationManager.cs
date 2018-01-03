using System.Linq;
using Microsoft.AspNetCore.Http;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Errors;
using Vokabular.MainService.Core.Managers.Authentication;
using Vokabular.MainService.Core.Works.Users;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Headers;

namespace Vokabular.MainService.Core.Managers
{
    public class AuthenticationManager
    {
        private readonly UserRepository m_userRepository;
        private readonly ICommunicationTokenGenerator m_communicationTokenGenerator;
        private readonly IHttpContextAccessor m_httpContextAccessor;
        private readonly DefaultUserProvider m_defaultUserProvider;

        public AuthenticationManager(UserRepository userRepository, ICommunicationTokenGenerator communicationTokenGenerator, IHttpContextAccessor httpContextAccessor, DefaultUserProvider defaultUserProvider)
        {
            m_userRepository = userRepository;
            m_communicationTokenGenerator = communicationTokenGenerator;
            m_httpContextAccessor = httpContextAccessor;
            m_defaultUserProvider = defaultUserProvider;
        }

        public User GetCurrentUser(bool returnDefaultIfNull)
        {
            if (!m_httpContextAccessor.HttpContext.Request.Headers.TryGetValue(CustomHttpHeaders.Authorization, out var communicationTokens))
            {
                if (returnDefaultIfNull)
                {
                    return m_defaultUserProvider.GetDefaultUser();
                }

                throw new AuthenticationException("User not signed in");
            }

            var communicationToken = communicationTokens.First();
            var user = m_userRepository.InvokeUnitOfWork(x => x.GetUserByToken(communicationToken));
            return user;
        }

        public int GetCurrentUserId()
        {
            return GetCurrentUser(false).Id;
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
}
