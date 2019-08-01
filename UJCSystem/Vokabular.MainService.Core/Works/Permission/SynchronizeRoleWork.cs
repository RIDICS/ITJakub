using System;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Communication;
using Vokabular.Shared.DataEntities.UnitOfWork;
using AuthRoleContract = Ridics.Authentication.DataContracts.RoleContract;

namespace Vokabular.MainService.Core.Works.Permission
{
    public class SynchronizeRoleWork : UnitOfWorkBase
    {
        private readonly PermissionRepository m_permissionRepository;
        private readonly CommunicationProvider m_communicationProvider;
        private readonly int m_roleId;
        private AuthRoleContract m_roleContract;

        private SynchronizeRoleWork(PermissionRepository permissionRepository, CommunicationProvider communicationProvider) : base(
            permissionRepository)
        {
            m_permissionRepository = permissionRepository;
            m_communicationProvider = communicationProvider;
        }

        public SynchronizeRoleWork(PermissionRepository permissionRepository, CommunicationProvider communicationProvider, int roleId) :
            this(permissionRepository, communicationProvider)
        {
            m_roleId = roleId;
        }

        public SynchronizeRoleWork(PermissionRepository permissionRepository, CommunicationProvider communicationProvider,
            AuthRoleContract authRoleContract) : this(permissionRepository, communicationProvider)
        {
            m_roleContract = authRoleContract;
        }

        protected override void ExecuteWorkImplementation()
        {
            if (m_roleContract == null)
            {
                m_roleContract = m_communicationProvider.GetAuthRoleApiClient().HttpClient.GetItemAsync<AuthRoleContract>(m_roleId).GetAwaiter()
                    .GetResult();
            }
            var group = m_permissionRepository.FindGroupByExternalIdOrCreate(m_roleContract.Id);

            if (group.Name != m_roleContract.Name)
            {
                group.Name = m_roleContract.Name;
                group.LastChange = DateTime.UtcNow;
                m_permissionRepository.Save(group);
                m_permissionRepository.Flush();
            }
        }

        public AuthRoleContract GetRoleContract()
        {
            return m_roleContract;
        }
    }
}