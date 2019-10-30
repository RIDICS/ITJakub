using System.Net;
using Ridics.Authentication.DataContracts;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.Core.Managers.Authentication;
using Vokabular.MainService.DataContracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Permission
{
    public class DeleteRoleWork : UnitOfWorkBase
    {
        private readonly PermissionRepository m_permissionRepository;
        private readonly DefaultUserProvider m_defaultUserProvider;
        private readonly CommunicationProvider m_communicationProvider;
        private readonly int m_roleId;

        public DeleteRoleWork(PermissionRepository permissionRepository, DefaultUserProvider defaultUserProvider,
            CommunicationProvider communicationProvider, int roleId) : base(permissionRepository)
        {
            m_permissionRepository = permissionRepository;
            m_defaultUserProvider = defaultUserProvider;
            m_communicationProvider = communicationProvider;
            m_roleId = roleId;
        }

        protected override void ExecuteWorkImplementation()
        {
            CheckRoleForDeleting(m_defaultUserProvider.GetDefaultUnregisteredRole());
            CheckRoleForDeleting(m_defaultUserProvider.GetDefaultRegisteredRole());

            var group = m_permissionRepository.FindById<UserGroup>(m_roleId);
            m_permissionRepository.Delete(group);
            m_permissionRepository.Flush();

            if (group is RoleUserGroup roleUserGroup)
            {
                var client = m_communicationProvider.GetAuthRoleApiClient();
                client.DeleteRoleAsync(roleUserGroup.ExternalId).GetAwaiter().GetResult();
            }
        }

        private void CheckRoleForDeleting(RoleContractBase defaultRole)
        {
            var dbRole = m_permissionRepository.FindById<RoleUserGroup>(m_roleId);

            if (dbRole != null && defaultRole.Id == dbRole.ExternalId)
            {
                throw new MainServiceException(MainServiceErrorCode.DeleteDefaultRole,
                    $"The default role {defaultRole.Name} cannot be deleted.",
                    HttpStatusCode.BadRequest,
                    defaultRole.Name
                );
            }
        }
    }
}