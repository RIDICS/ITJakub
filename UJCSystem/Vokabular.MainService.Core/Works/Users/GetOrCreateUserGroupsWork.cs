using System.Collections.Generic;
using Ridics.Authentication.DataContracts;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Users
{
    public class GetOrCreateUserGroupsWork : UnitOfWorkBase<IList<UserGroup>>
    {
        private readonly UserRepository m_userRepository;
        private readonly IList<RoleContractBase> m_authRoles;

        public GetOrCreateUserGroupsWork(UserRepository userRepository, IList<RoleContractBase> authRoles) : base(userRepository)
        {
            m_userRepository = userRepository;
            m_authRoles = authRoles;
        }

        protected override IList<UserGroup> ExecuteWorkImplementation()
        {
            var subwork = new UserGroupSubwork(m_userRepository);
            var result = subwork.UpdateAndGetUserGroups(m_authRoles);
            return result;
        }
    }
}