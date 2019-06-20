using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Vokabular.Authentication.Client.Configuration;
using Vokabular.Authentication.DataContracts;
using Vokabular.Authentication.DataContracts.User;

namespace Vokabular.Authentication.Client.Client.Auth
{
    public class UserApiClient : BaseApiClient
    {
        public UserApiClient(
            AuthorizationServiceHttpClient authorizationServiceHttpClient,
            AuthServiceControllerBasePathsConfiguration basePathsConfiguration
        ) : base(authorizationServiceHttpClient, basePathsConfiguration)
        {
        }

        protected override string BasePath => m_basePathsConfiguration.UserBasePath;

        public async Task<HttpResponseMessage> EditSelfAsync(int id, UserContract content)
        {
            var fullPath = $"{BasePath}{id}/editself";
            return await m_authorizationServiceHttpClient.SendRequestAsync(HttpMethod.Put, fullPath, content);
        }

        public async Task<HttpResponseMessage> AssignRolesToUserAsync(int id, List<int> selectedRoles)
        {
            var fullPath = $"{BasePath}{id}/Roles";
            return await m_authorizationServiceHttpClient.SendRequestAsync(HttpMethod.Post, fullPath, selectedRoles);
        }

        public async Task<HttpResponseMessage> AddRoleToUserAsync(int id, int roleId)
        {
            var fullPath = $"{BasePath}{id}/role/{roleId}";
            return await m_authorizationServiceHttpClient.SendRequestAsync(HttpMethod.Put, fullPath);
        }

        public async Task<HttpResponseMessage> RemoveRoleFromUserAsync(int id, int roleId)
        {
            var fullPath = $"{BasePath}{id}/role/{roleId}";
            return await m_authorizationServiceHttpClient.SendRequestAsync(HttpMethod.Delete, fullPath);
        }

        public async Task<HttpResponseMessage> DisconnectExternalLoginAsync(int id, int externalLoginId)
        {
            var fullPath = $"{BasePath}{id}/disconnectExternalLogin";

            var requestContract = new ExternalLoginContract
            {
                Id = externalLoginId
            };

            return await m_authorizationServiceHttpClient.SendRequestAsync(HttpMethod.Post, fullPath, requestContract);
        }

        public async Task<HttpResponseMessage> PasswordChangeAsync(int userId, ChangePasswordContract contract)
        {
            var fullPath = $"{BasePath}{userId}/changePassword";
            return await m_authorizationServiceHttpClient.SendRequestAsync(HttpMethod.Post, fullPath, contract);
        }

        public async Task<bool> ChangeContactAsync(int userId, ChangeUserContactsContract contract)
        {
            var fullPath = $"{BasePath}{userId}/changeContact";
            return await m_authorizationServiceHttpClient.SendRequestAsync<bool>(HttpMethod.Post, fullPath, contract);
        }

        public async Task<HttpResponseMessage> SetTwoFactorAsync(int userId, ChangeTwoFactorContract contract)
        {
            var fullPath = $"{BasePath}{userId}/setTwoFactor";
            return await m_authorizationServiceHttpClient.SendRequestAsync(HttpMethod.Post, fullPath, contract);
        }

        public async Task<HttpResponseMessage> SelectTwoFactorProviderAsync(int userId, ChangeTwoFactorContract contract)
        {
            var fullPath = $"{BasePath}{userId}/changeTwoFactorProvider";
            return await m_authorizationServiceHttpClient.SendRequestAsync(HttpMethod.Post, fullPath, contract);
        }

        public async Task CreateVerificationRequestAsync(int userId)
        {
            var fullPath = $"{BasePath}{userId}/createVerificationRequest";
            await m_authorizationServiceHttpClient.SendRequestAsync(HttpMethod.Post, fullPath);
        }
        
        public async Task<IList<BasicUserInfoContract>> GetBasicUsersInfoAsync(IList<Guid> muids)
        {
            var query = m_authorizationServiceHttpClient.CreateQueryCollection();
            for (var i = 0; i < muids.Count; i++)
            {
                query.Add($"muids[{i}]", muids[i].ToString());
            }
            var path = $"{BasePath}basic/list?{query}";
            var response = await m_authorizationServiceHttpClient.SendRequestAsync<IList<BasicUserInfoContract>>(HttpMethod.Get, path);
            return response;
        }

        public async Task<BasicUserInfoContract> GetBasicUserInfoAsync(UserIdentifierTypeContract idType, string id)
        {
            var query = m_authorizationServiceHttpClient.CreateQueryCollection();
            query.Add("idType", idType.ToString());
            query.Add("id", id);
            var path = $"{BasePath}basic?{query}";
            var response = await m_authorizationServiceHttpClient.SendRequestAsync<BasicUserInfoContract>(HttpMethod.Get, path);
            return response;
        }

        public async Task<IList<UserContactContract>> GetUserContacts(UserIdentifierTypeContract userIdType, string idValue)
        {
            var query = m_authorizationServiceHttpClient.CreateQueryCollection();
            query.Add("userIdType", userIdType.ToString());
            query.Add("idValue", idValue);
            var path = $"{BasePath}contact?{query}";
            var response = await m_authorizationServiceHttpClient.SendRequestAsync<IList<UserContactContract>>(HttpMethod.Get, path);
            return response;
        }

        public async Task<UserContractBase> GetUserByMuidAsync(Guid muid)
        {
            var path = $"{BasePath}{muid}/full";

            var response = await m_authorizationServiceHttpClient.SendRequestAsync<UserContractBase>(HttpMethod.Get, path);
            return response;
        }

        public async Task<UserWithRolesContract> GetUserForRoleAssignmentAsync(int userId)
        {
            var path = $"{BasePath}{userId}/basic/roles";
            var response = await m_authorizationServiceHttpClient.SendRequestAsync<UserWithRolesContract>(HttpMethod.Get, path);
            return response;
        }

        public async Task<IList<RoleContractBase>> GetRolesByUserAsync(int userId)
        {
            var path = $"{BasePath}{userId}/role";
            var response = await m_authorizationServiceHttpClient.SendRequestAsync<IList<RoleContractBase>>(HttpMethod.Get, path);
            return response;
        }

        public async Task<string> GetUserMuidAsync(UserIdentifierTypeContract idType, string id)
        {
            var query = m_authorizationServiceHttpClient.CreateQueryCollection();
            query.Add("idType", idType.ToString());
            query.Add("id", id);
            var path = $"{BasePath}muid?{query}";
            var response = await m_authorizationServiceHttpClient.SendRequestAsync<string>(HttpMethod.Get, path);
            return response;
        }
    }
}