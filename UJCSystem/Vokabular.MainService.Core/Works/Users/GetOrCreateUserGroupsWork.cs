using System.Collections.Generic;
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

        public GetOrCreateUserGroupsWork(UserRepository userRepository, IList<T> authRoles) : base(userRepository)
        {
            m_userRepository = userRepository;
            m_authRoles = authRoles;
        }

        protected override IList<RoleUserGroup> ExecuteWorkImplementation()
        {
            var subwork = new UserGroupSubwork(m_userRepository);
            var result = subwork.UpdateAndGetUserGroups(m_authRoles);
            return result;
        }
    }
}