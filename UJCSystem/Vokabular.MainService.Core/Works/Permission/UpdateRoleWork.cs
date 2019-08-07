using System;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.Core.Managers.Authentication;
using Vokabular.Shared.DataEntities.UnitOfWork;
using AuthRoleContract = Ridics.Authentication.DataContracts.RoleContract;
using RoleContract = Vokabular.MainService.DataContracts.Contracts.Permission.RoleContract;

namespace Vokabular.MainService.Core.Works.Permission
{
    public class UpdateRoleWork : UnitOfWorkBase
    {
        private readonly PermissionRepository m_permissionRepository;
        private readonly DefaultUserProvider m_defaultUserProvider;
        private readonly CommunicationProvider m_communicationProvider;
        private readonly RoleContract m_data;

        public UpdateRoleWork(PermissionRepository permissionRepository, DefaultUserProvider defaultUserProvider,
            CommunicationProvider communicationProvider, RoleContract data) : base(permissionRepository)
        {
            m_permissionRepository = permissionRepository;
            m_defaultUserProvider = defaultUserProvider;
            m_data = data;
            m_communicationProvider = communicationProvider;
        }

        protected override void ExecuteWorkImplementation()
        {
            var role = m_defaultUserProvider.GetDefaultUnregisteredRole();
            if (role.Id == m_data.Id && role.Name != m_data.Name)
            {
                throw new ArgumentException($"The name of the default role {role.Name} cannot be changed.");
            }

            var group = m_permissionRepository.FindGroupByExternalIdOrCreate(m_data.Id);
            group.Name = m_data.Name;
            group.LastChange = DateTime.UtcNow;
            m_permissionRepository.Save(group);
            m_permissionRepository.Flush();

            var client = m_communicationProvider.GetAuthRoleApiClient();
            var authRole = client.HttpClient.GetItemAsync<AuthRoleContract>(m_data.Id).GetAwaiter().GetResult();
            authRole.Name = m_data.Name;
            authRole.Description = m_data.Description;

            client.HttpClient.EditItemAsync(m_data.Id, authRole).GetAwaiter().GetResult();
        }
    }
}