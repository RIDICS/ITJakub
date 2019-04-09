using System.Linq;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.ProjectImport.Works
{
    public class ProcessExternalImportPermissionWork : UnitOfWorkBase
    {
        private readonly PermissionRepository m_permissionRepository;
        private readonly long m_projectId;

        public ProcessExternalImportPermissionWork(PermissionRepository permissionRepository, long projectId) : base(permissionRepository)
        {
            m_permissionRepository = permissionRepository;
            m_projectId = projectId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var specialPermissions = m_permissionRepository.GetSpecialPermissions();
            var importPermissions = specialPermissions.OfType<ReadExternalProjectPermission>();

            var groupsWithPermission = m_permissionRepository.GetGroupsBySpecialPermissionIds(importPermissions.Select(x => x.Id));
            var project = m_permissionRepository.Load<Project>(m_projectId);

            var newPermissions = groupsWithPermission.Select(groupWithPermission => new Permission
            {
                Project = project,
                UserGroup = groupWithPermission
            });

            foreach (var newPermission in newPermissions)
            {
                m_permissionRepository.CreatePermissionIfNotExist(newPermission);
            }
        }
    }
}