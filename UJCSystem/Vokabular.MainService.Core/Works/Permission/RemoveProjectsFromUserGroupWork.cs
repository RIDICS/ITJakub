using System.Collections.Generic;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Permission
{
    public class RemoveProjectsFromUserGroupWork : UnitOfWorkBase
    {
        private readonly PermissionRepository m_permissionRepository;
        private readonly int m_roleId;
        private readonly IList<long> m_bookIds;

        public RemoveProjectsFromUserGroupWork(PermissionRepository permissionRepository, int roleId, IList<long> bookIds) : base(permissionRepository)
        {
            m_permissionRepository = permissionRepository;
            m_roleId = roleId;
            m_bookIds = bookIds;
        }

        protected override void ExecuteWorkImplementation()
        {
            var allBookIds = new List<long>();

            //if (categoryIds != null && categoryIds.Count > 0)
            //{
            //    var bookIdsFromCategories = m_categoryRepository.GetBookIdsFromCategory(categoryIds);
            //    allBookIds.AddRange(bookIdsFromCategories);
            //}

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