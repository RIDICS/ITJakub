using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Permission
{
    public class DeleteGroupWork : UnitOfWorkBase
    {
        private readonly PermissionRepository m_permissionRepository;
        private readonly int m_groupId;

        public DeleteGroupWork(PermissionRepository permissionRepository, int groupId) : base(permissionRepository)
        {
            m_permissionRepository = permissionRepository;
            m_groupId = groupId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var group = m_permissionRepository.FindById<UserGroup>(m_groupId);
            m_permissionRepository.Delete(group);
        }
    }
}