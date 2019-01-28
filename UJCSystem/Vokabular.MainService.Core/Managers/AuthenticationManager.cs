using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using System.Security.Authentication;
using System.Security.Claims;
using Vokabular.MainService.Core.Managers.Authentication;
using Vokabular.Shared.AspNetCore.Extensions;
using Vokabular.Shared.Const;

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
                var externalUser = m_httpContextAccessor.HttpContext.User;
                var user = m_userRepository.InvokeUnitOfWork(x => x.GetUserByExternalId(externalUser.GetId()));

                if (user == null)
                {
                    throw new AuthenticationException("Invalid external user id.");
                }

                return user;
            }
            catch (AuthenticationException)
            {
                if (returnDefaultIfNull)
                {
                    return m_defaultUserProvider.GetDefaultUser();
                }

                throw;
            }
        }

        public User GetCurrentUser()
        {
            var externalUser = m_httpContextAccessor.HttpContext.User;
            var user = m_userRepository.InvokeUnitOfWork(x => x.GetUserByExternalId(externalUser.GetId()));

            if (user == null)
            {
                throw new AuthenticationException("Invalid external user id.");
            }

            return user;
        }

        public int GetCurrentUserId()
        {
            return GetCurrentUser().Id;
        }

        public IList<Claim> GetCurrentUserPermissions(bool returnDefaultIfNull)
        {
            if (m_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated || returnDefaultIfNull == false)
            {
                return m_httpContextAccessor.HttpContext.User.Claims.Where(x => x.Type == CustomClaimTypes.Permission).ToList();
            }

            return m_defaultUserProvider.GetDefaultUserPermissions();
        }
    }
}