using System;
using Vokabular.Authentication.DataContracts;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Communication;

namespace Vokabular.MainService.Core.Works.Permission
{
    public class CreateRoleWork : UnitOfWorkBase<int>
    {
        private readonly PermissionRepository m_permissionRepository;
        private readonly CommunicationProvider m_communicationProvider;
        private readonly string m_roleName;
        private readonly string m_description;
        private readonly int m_userId;

        public CreateRoleWork(PermissionRepository permissionRepository, CommunicationProvider communicationProvider, string roleName, string description, int userId) : base(permissionRepository)
        {
            m_permissionRepository = permissionRepository;
            m_communicationProvider = communicationProvider;
            m_roleName = roleName;
            m_description = description;
            m_userId = userId;
        }

        protected override int ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;
            var user = m_permissionRepository.Load<User>(m_userId);

            using (var client = m_communicationProvider.GetAuthenticationServiceClient())
            {
                var roleContract = new RoleContract
                {
                    Description = m_description,
                    Name = m_roleName,
                };

                var roleId = client.CreateRole(roleContract);

                var group = new UserGroup
                {
                    Name = m_roleName,
                    Description = m_description,
                    CreateTime = now,
                    CreatedBy = user,
                    ExternalId = roleId
                };

                m_permissionRepository.CreateGroup(group);
                return roleId;
            }
        }
    }
}
