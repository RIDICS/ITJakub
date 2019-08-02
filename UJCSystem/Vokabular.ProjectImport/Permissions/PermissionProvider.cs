using System.Linq;
using Ridics.Authentication.DataContracts;
using Ridics.Authentication.HttpClient.Client.Auth;

namespace Vokabular.ProjectImport.Permissions
{
    public class PermissionProvider : IPermissionsProvider
    {
        private readonly PermissionApiClient m_permissionApiClient;

        public PermissionProvider(PermissionApiClient permissionApiClient)
        {
            m_permissionApiClient = permissionApiClient;
        }

        public PermissionContract GetPermissionByName(string name)
        {
            var allPermissions = m_permissionApiClient.GetAllPermissionsAsync().GetAwaiter().GetResult();
            var permission =
                allPermissions.SingleOrDefault(x => x.Name == name);

            return permission;
        }
    }
}