using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Vokabular.Authentication.Client.Configuration;
using Vokabular.Authentication.DataContracts;

namespace Vokabular.Authentication.Client.Client.Auth
{
    public class RoleApiClient : BaseApiClient
    {
        public RoleApiClient(
            AuthorizationServiceHttpClient authorizationServiceHttpClient,
            AuthServiceControllerBasePathsConfiguration basePathsConfiguration
        ) : base(authorizationServiceHttpClient, basePathsConfiguration)
        {
        }

        protected override string BasePath => m_basePathsConfiguration.RoleBasePath;

        public async Task AssignPermissionsToRoleAsync(int id, IEnumerable<int> selectedPermissions)
        {
            var fullPath = $"{BasePath}{id}/Permissions";
            await m_authorizationServiceHttpClient.SendRequestAsync(HttpMethod.Post, fullPath, selectedPermissions);
        }

        public async Task<IList<RoleContract>> GetAllRolesAsync()
        {
            var fullPath = $"{BasePath}AllRoles";
            return await m_authorizationServiceHttpClient.SendRequestAsync<IList<RoleContract>>(HttpMethod.Get, fullPath);
        }
    }
}