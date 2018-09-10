using System.Collections.Generic;
using System.Reflection;
using log4net;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Permission
{
    public class RemoveSpecialPermissionsFromGroupWork : UnitOfWorkBase
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly PermissionRepository m_permissionRepository;
        private readonly int m_groupId;
        private readonly IList<int> m_specialPermissionsIds;

        public RemoveSpecialPermissionsFromGroupWork(PermissionRepository permissionRepository, int groupId, IList<int> specialPermissionsIds) : base(permissionRepository)
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
                if (m_log.IsWarnEnabled)
                {
                    string message = string.Format("Cannot remove special permissions from group with id '{0}'. Group special permissions are empty.", group.Id);
                    m_log.Warn(message);
                }
                return;
            }

            foreach (var specialPermission in specialPermissions)
            {
                group.SpecialPermissions.Remove(specialPermission);
            }

            m_permissionRepository.Save(group);
        }
    }
}