using System.Collections.Generic;
using System.Linq;
using Ridics.Authentication.HttpClient.Client.Auth;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ProjectImport.Permissions
{
    public class PermissionProvider : IPermissionsProvider
    {
        private readonly PermissionApiClient m_permissionApiClient;
        private readonly PermissionRepository m_permissionRepository;

        public PermissionProvider(PermissionApiClient permissionApiClient, PermissionRepository permissionRepository)
        {
            m_permissionApiClient = permissionApiClient;
            m_permissionRepository = permissionRepository;
        }

        public IList<int> GetRoleIdsByPermissionName(string name)
        {
            var allPermissions = m_permissionApiClient.GetAllPermissionsAsync(name).GetAwaiter().GetResult();
            var permission = allPermissions.SingleOrDefault(x => x.Name == name);
            if (permission == null)
            {
                return null;
            }

            var roleExternalIds = m_permissionApiClient.GetRoleIdsByPermissionAsync(permission.Id).GetAwaiter().GetResult();
            var roleIds = m_permissionRepository.InvokeUnitOfWork(x => x.GetGroupIdsByExternalIds(roleExternalIds));

            return roleIds;
        }
    }
}