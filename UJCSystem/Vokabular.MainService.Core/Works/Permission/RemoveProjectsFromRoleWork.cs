using System.Collections.Generic;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Permission
{
    public class RemoveProjectsFromRoleWork : UnitOfWorkBase
    {
        private readonly PermissionRepository m_permissionRepository;
        private readonly int m_roleId;
        private readonly IList<long> m_bookIds;
        private readonly ProjectPermissionsSubwork m_permissionsSubwork;

        public RemoveProjectsFromRoleWork(PermissionRepository permissionRepository, int roleId, IList<long> bookIds) : base(permissionRepository)
        {
            m_permissionRepository = permissionRepository;
            m_roleId = roleId;
            m_bookIds = bookIds;
            m_permissionsSubwork = new ProjectPermissionsSubwork(permissionRepository);
        }

        protected override void ExecuteWorkImplementation()
        {
            var allBookIds = new List<long>();

            if (m_bookIds != null)
            {
                allBookIds.AddRange(m_bookIds);
            }

            var permissions = m_permissionRepository.FindPermissionsByGroupAndBooks(m_roleId, allBookIds);
            m_permissionRepository.DeleteAll(permissions);

            foreach (var bookId in allBookIds)
            {
                m_permissionsSubwork.CheckRemainingAdministrator(bookId);
            }
        }
    }
}