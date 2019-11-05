using System.Collections.Generic;
using System.Linq;
using Ridics.Authentication.DataContracts;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Users
{
    public class GetOrCreateUserGroupsWork<T> : UnitOfWorkBase<IList<RoleUserGroup>> where T : RoleContractBase
    {
        private readonly UserRepository m_userRepository;
        private readonly IList<T> m_authRoles;
        private readonly int? m_userId;

        public GetOrCreateUserGroupsWork(UserRepository userRepository, IList<T> authRoles, int? userId = null) : base(userRepository)
        {
            m_userRepository = userRepository;
            m_authRoles = authRoles;
            m_userId = userId;
        }

        protected override IList<RoleUserGroup> ExecuteWorkImplementation()
        {
            var subwork = new UserGroupSubwork(m_userRepository);
            var result = subwork.UpdateAndGetUserGroups(m_authRoles);

            if (m_userId != null)
            {
                // Update assigned RoleUserGroups for user
                var user = m_userRepository.GetUserById(m_userId.Value);
                var nonRoleGroups = user.Groups.Where(x => !(x is RoleUserGroup));

                var newUserGroups = new List<UserGroup>();
                newUserGroups.AddRange(nonRoleGroups);
                newUserGroups.AddRange(result);

                user.Groups = newUserGroups;
            }

            return result;
        }
    }
}