using System.Collections.Generic;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Permission
{
    public class RemoveProjectsFromRoleWork : UnitOfWorkBase
    {
        private readonly PermissionRepository m_permissionRepository;
        private readonly int m_roleId;
        private readonly IList<long> m_bookIds;

        public RemoveProjectsFromRoleWork(PermissionRepository permissionRepository, int roleId, IList<long> bookIds) : base(permissionRepository)
        {
            m_permissionRepository = permissionRepository;
            m_roleId = roleId;
            m_bookIds = bookIds;
        }

        protected override void ExecuteWorkImplementation()
        {
            var allBookIds = new List<long>();

            if (m_bookIds != null)
            {
                allBookIds.AddRange(m_bookIds);
            }

            var group = m_permissionRepository.FindGroupByExternalIdOrCreate(m_roleId);

            var permissions = m_permissionRepository.FindPermissionsByGroupAndBooks(group.Id, allBookIds);
            m_permissionRepository.DeleteAll(permissions);
        }
    }
}