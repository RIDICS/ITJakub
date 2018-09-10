using System.Collections.Generic;
using System.Linq;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Permission
{
    public class AddSpecialPermissionsToGroupWork : UnitOfWorkBase
    {
        private readonly PermissionRepository m_permissionRepository;
        private readonly int m_groupId;
        private readonly IList<int> m_specialPermissionsIds;

        public AddSpecialPermissionsToGroupWork(PermissionRepository permissionRepository, int groupId, IList<int> specialPermissionsIds) : base(permissionRepository)
        {
            m_permissionRepository = permissionRepository;
            m_groupId = groupId;
            m_specialPermissionsIds = specialPermissionsIds;
        }

        protected override void ExecuteWorkImplementation()
        {
            var specialPermissions = m_permissionRepository.GetSpecialPermissionsByIds(m_specialPermissionsIds);
            var group = m_permissionRepository.FindGroupWithSpecialPermissionsById(m_groupId);

            if (group.SpecialPermissions == null)
            {
                group.SpecialPermissions = new List<SpecialPermission>();
            }

            var missingSpecialPermissions = specialPermissions.Where(x => !group.SpecialPermissions.Contains(x));

            foreach (var specialPermission in missingSpecialPermissions)
            {
                group.SpecialPermissions.Add(specialPermission);
            }

            m_permissionRepository.Save(group);
        }
    }
}
