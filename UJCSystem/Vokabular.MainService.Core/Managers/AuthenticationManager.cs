using Microsoft.AspNetCore.Http;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using System.Security.Authentication;
using Vokabular.MainService.Core.Managers.Authentication;
using Vokabular.Shared.AspNetCore.Extensions;

namespace Vokabular.MainService.Core.Managers
{
    public class AuthenticationManager
    {
        private readonly UserRepository m_userRepository;
        private readonly DefaultUserProvider m_defaultUserProvider;
        private readonly IHttpContextAccessor m_httpContextAccessor;

        public AuthenticationManager(UserRepository userRepository, DefaultUserProvider defaultUserProvider,
            IHttpContextAccessor httpContextAccessor)
        {
            m_userRepository = userRepository;
            m_defaultUserProvider = defaultUserProvider;
            m_httpContextAccessor = httpContextAccessor;
        }

        public User GetCurrentUser(bool returnDefaultIfNull)
        {
            try
            {
                var externalUserId = m_httpContextAccessor.HttpContext.User.GetId();
                var user = m_userRepository.InvokeUnitOfWork(x => x.GetUserByExternalId(externalUserId));

                if (user == null)
                {
                    throw new AuthenticationException("Invalid external user id.");
                }

                return user;
            }
            catch (AuthenticationException e)
            {
                if (returnDefaultIfNull)
                {
                    return m_defaultUserProvider.GetDefaultUser();
                }

                throw;
            }
        }

        public int GetCurrentUserId()
        {
            return GetCurrentUser(false).Id;
        }
    }
}