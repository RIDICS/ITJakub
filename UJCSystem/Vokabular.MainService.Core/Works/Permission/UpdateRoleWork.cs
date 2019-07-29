using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Communication;
using Vokabular.Shared.DataEntities.UnitOfWork;
using AuthRoleContract = Ridics.Authentication.DataContracts.RoleContract;
using RoleContract = Vokabular.MainService.DataContracts.Contracts.Permission.RoleContract;

namespace Vokabular.MainService.Core.Works.Permission
{
    public class UpdateRoleWork : UnitOfWorkBase
    {
        private readonly PermissionRepository m_permissionRepository;
        private readonly CommunicationProvider m_communicationProvider;
        private readonly RoleContract m_data;

        public UpdateRoleWork(PermissionRepository permissionRepository, RoleContract data, CommunicationProvider communicationProvider) : base(permissionRepository)
        {
            m_permissionRepository = permissionRepository;
            m_data = data;
            m_communicationProvider = communicationProvider;
        }

        protected override void ExecuteWorkImplementation()
        {
            var group = m_permissionRepository.FindGroupByExternalIdOrCreate(m_data.Id);
            group.Name = m_data.Name;
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
