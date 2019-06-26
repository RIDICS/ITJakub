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

        public async Task<ListContract<UserWithRolesContract>> GetUserListByRoleAsync(int roleId, int? start, int? count, string search)
        {
            var query = m_authorizationServiceHttpClient.CreateQueryCollection();

            if (start != null) query.Add("start", start.Value.ToString());
            if (count != null) query.Add("count", count.Value.ToString());
            if (!string.IsNullOrEmpty(search)) query.Add("search", search);

            var path = $"{BasePath}{roleId}/Users?{query}";
            var response = await m_authorizationServiceHttpClient.SendRequestAsync<ListContract<UserWithRolesContract>>(HttpMethod.Get, path);

            return response;
        }

        public async Task UpdateRole(int id, RoleContract roleContract)
        {
            var fullPath = $"{BasePath}{id}/edit";
            await m_authorizationServiceHttpClient.SendRequestAsync(HttpMethod.Post, fullPath, roleContract);
        }

    }
}