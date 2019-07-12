using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using System.Security.Claims;
using Ridics.Authentication.DataContracts;
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

        public User GetCurrentUser(bool returnDefaultUserIfNull)
        {
            var id = m_httpContextAccessor.HttpContext.User.GetId();

            if (id.HasValue)
            {
                return m_userRepository.InvokeUnitOfWork(x => x.GetUserByExternalId(id.Value));
            }

            if (returnDefaultUserIfNull)
            {
                return m_defaultUserProvider.GetDefaultUser();
            }

            return null;
        }

        public User GetCurrentUser()
        {
            return GetCurrentUser(false);
        }

        public RoleContract GetUnregisteredRole()
        {
            return m_defaultUserProvider.GetDefaultUnregisteredRole();
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