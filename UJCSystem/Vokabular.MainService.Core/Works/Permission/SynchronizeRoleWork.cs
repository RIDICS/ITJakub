using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using AuthRoleContract = Ridics.Authentication.DataContracts.RoleContract;

namespace Vokabular.MainService.Core.Works.Permission
{
    public class SynchronizeRoleWork : UnitOfWorkBase
    {
        private readonly PermissionRepository m_permissionRepository;
        private readonly AuthRoleContract m_data;

        public SynchronizeRoleWork(PermissionRepository permissionRepository, AuthRoleContract data) : base(permissionRepository)
        {
            m_permissionRepository = permissionRepository;
            m_data = data;
        }

        protected override void ExecuteWorkImplementation()
        {
            var group = m_permissionRepository.FindGroupByExternalIdOrCreate(m_data.Id);
            group.Name = m_data.Name;
            m_permissionRepository.Save(group);
            m_permissionRepository.Flush();
        }
    }
}
