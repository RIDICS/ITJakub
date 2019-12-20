using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts;

namespace Vokabular.MainService.Core.Works.Permission
{
    public class ProjectPermissionsSubwork
    {
        private readonly PermissionRepository m_permissionRepository;

        public ProjectPermissionsSubwork(PermissionRepository permissionRepository)
        {
            m_permissionRepository = permissionRepository;
        }

        public void CheckRemainingAdministrator(long projectId)
        {
            var project = m_permissionRepository.FindById<Project>(projectId);
            if (project.ProjectType == ProjectTypeEnum.Research || project.ProjectType == ProjectTypeEnum.Bibliography)
            {
                // No check for books in Research portal or for bibliography items
                return;
            }

            m_permissionRepository.Flush();

            var adminPermissionCount = m_permissionRepository.GetRequiredPermissionCountForProject(projectId, PermissionFlag.AdminProject);
            if (adminPermissionCount == 0)
            {
                throw new MainServiceException(MainServiceErrorCode.AtLeastOneGroupMustHaveAdminPermission, "No remaining group assigned to this project contains required Admin permission");
            }
        }

        public void CheckPermissionConsistency(PermissionFlag permission)
        {
            if ((permission.HasFlag(PermissionFlag.EditProject) || permission.HasFlag(PermissionFlag.AdminProject)) && !permission.HasFlag(PermissionFlag.ReadProject))
            {
                throw new MainServiceException(MainServiceErrorCode.ReadProjectRequiredForSelectedPermissions, "EditProject and AdminProject permissions require assigned ReadProject permission");
            }
        }
    }
}