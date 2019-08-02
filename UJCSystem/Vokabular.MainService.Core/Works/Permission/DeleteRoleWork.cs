using Ridics.Authentication.DataContracts;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Communication;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Permission
{
    public class DeleteRoleWork : UnitOfWorkBase
    {
        private readonly PermissionRepository m_permissionRepository;
        private readonly CommunicationProvider m_communicationProvider;
        private readonly int m_roleId;

        public DeleteRoleWork(PermissionRepository permissionRepository, CommunicationProvider communicationProvider, int roleId) : base(permissionRepository)
        {
            m_permissionRepository = permissionRepository;
            m_communicationProvider = communicationProvider;
            m_roleId = roleId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var group = m_permissionRepository.FindGroupByExternalId(m_roleId);
            if (group != null)
            {
                m_permissionRepository.Delete(group);
                m_permissionRepository.Flush();
            }
            
            var client = m_communicationProvider.GetAuthRoleApiClient();
            client.HttpClient.DeleteItemAsync<RoleContract>(m_roleId).GetAwaiter().GetResult();
        }
    }
}
