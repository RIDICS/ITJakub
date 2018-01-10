using System.Collections.Generic;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Permission
{
    public class RemoveProjectsFromUserGroupWork : UnitOfWorkBase
    {
        private readonly PermissionRepository m_permissionRepository;
        private readonly int m_groupId;
        private readonly IList<long> m_bookIds;

        public RemoveProjectsFromUserGroupWork(PermissionRepository permissionRepository, int groupId, IList<long> bookIds) : base(permissionRepository)
        {
            m_permissionRepository = permissionRepository;
            m_groupId = groupId;
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

            var permissions = m_permissionRepository.FindPermissionsByGroupAndBooks(m_groupId, allBookIds);
            m_permissionRepository.DeleteAll(permissions);
        }
    }
}