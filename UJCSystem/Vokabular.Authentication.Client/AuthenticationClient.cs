using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Vokabular.Authentication.DataContracts;
using Vokabular.Authentication.DataContracts.User;
using Vokabular.RestClient;
using Vokabular.Shared;
using Vokabular.Shared.Extensions;

namespace Vokabular.Authentication.Client
{
    public class AuthenticationClient : FullRestClientBase
    {
        private static readonly ILogger m_logger = ApplicationLogging.CreateLogger<AuthenticationClient>();
        private readonly string ApiBasePath = "api/v1";

        public AuthenticationClient(Uri baseAddress, string username, string password) : base(baseAddress, true)
        {
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(
                    System.Text.Encoding.ASCII.GetBytes($"{username}:{password}")));
        }

        protected override void FillRequestMessage(HttpRequestMessage requestMessage)
        {
        }

        protected override void ProcessResponse(HttpResponseMessage response)
        {
        }

        #region User

        public UserContract CreateUser(CreateUserContract contract)
        {
            try
            {
                var result = Post<UserContract>($"{ApiBasePath}/registration/create", contract);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public UserContract GetUser(int id)
        {
            try
            {
                var result = Get<UserContract>($"{ApiBasePath}/user/{id}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void EditCurrentUser(int userId, UserContract userContract)
        {
            try
            {
                Put<object>($"{ApiBasePath}/user/{userId}/editself", userContract);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void ChangePassword(int userId, ChangePasswordContract changePasswordContract)
        {
            try
            {
                Post<object>($"{ApiBasePath}/user/{userId}/changePassword", changePasswordContract);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public IList<UserContract> GetUserAutocomplete(string query, int? start, int? count)
        {
            try
            {
                return Get<IList<UserContract>>(
                    $"{ApiBasePath}/user/search?nameStart={query}{(start.HasValue ? "&start=" + start : "")}{(count.HasValue ? "&count=" + count : "")}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public IList<UserContract> GetUsersByRole(int roleId)
        {
            try
            {
                return Get<IList<UserContract>>($"{ApiBasePath}/user/roles/list?roleId={roleId}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        #endregion

        #region Roles

        public IList<RoleContract> GetAllRoles()
        {
            try
            {
                return Get<IList<RoleContract>>($"{ApiBasePath}/role/allroles");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public RoleContract GetRole(int roleId)
        {
            try
            {
                return Get<RoleContract>($"{ApiBasePath}/role/{roleId}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public int CreateRole(RoleContract roleContract)
        {
            try
            {
                return Post<int>($"{ApiBasePath}/role/create/", roleContract);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void DeleteRole(int roleId)
        {
            try
            {
                Delete($"{ApiBasePath}/role/{roleId}/delete");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void RemoveRoleFromUser(int userId, int roleId)
        {
            try
            {
                Delete($"{ApiBasePath}/user/{userId}/role/{roleId}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void AddRoleToUser(int userId, int roleId)
        {
            try
            {
                Put<object>($"{ApiBasePath}/user/{userId}/role/{roleId}", null);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public ListContract<RoleContract> GetListRole(string query, int count)
        {
            try
            {
                return Get<ListContract<RoleContract>>($"{ApiBasePath}/role/list?search={query}&count={count}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void AssignPermissionsToRole(int roleId, IEnumerable<int> permissions)
        {
            try
            {
                Post<IEnumerable<int>>($"{ApiBasePath}/role/{roleId}/permissions", permissions);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        #endregion

        #region Permissions

        public IList<PermissionContract> GetAllPermissions()
        {
            try
            {
                return Get<IList<PermissionContract>>($"{ApiBasePath}/permission/allpermissions");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        #endregion
    }
}