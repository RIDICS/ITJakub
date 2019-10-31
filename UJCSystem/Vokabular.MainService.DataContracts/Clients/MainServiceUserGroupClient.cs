using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Permission;
using Vokabular.RestClient;
using Vokabular.RestClient.Extensions;
using Vokabular.RestClient.Results;
using Vokabular.Shared;
using Vokabular.Shared.Extensions;

namespace Vokabular.MainService.DataContracts.Clients
{
    public class MainServiceUserGroupClient
    {
        private static readonly ILogger m_logger = ApplicationLogging.CreateLogger<MainServiceRestClient>();
        private readonly MainServiceRestClient m_client;

        public MainServiceUserGroupClient(MainServiceRestClient client)
        {
            m_client = client;
        }

        public PagedResultList<UserContract> GetUsersByGroup(int groupId, int start, int count, string query)
        {
            try
            {
                var url = UrlQueryBuilder.Create($"usergroup/{groupId}/user")
                    .AddParameter("start", start)
                    .AddParameter("count", count)
                    .AddParameter("filterByName", query)
                    .ToResult();
                
                var result = m_client.GetPagedList<UserContract>(url);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void AddUserToRole(int userId, int groupId)
        {
            try
            {
                m_client.Post<object>($"role/{groupId}/user/{userId}", null);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void RemoveUserFromRole(int userId, int groupId)
        {
            try
            {
                m_client.Delete($"role/{groupId}/user/{userId}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public PagedResultList<RoleContract> GetRoleList(int start, int count, string query)
        {
            try
            {
                var url = "role".AddQueryString("start", start.ToString());
                url = url.AddQueryString("count", count.ToString());
                url = url.AddQueryString("filterByName", query);
                var result = m_client.GetPagedList<RoleContract>(url);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public List<RoleContract> GetRoleAutocomplete(string query)
        {
            try
            {
                var result = m_client.Get<List<RoleContract>>("role/autocomplete".AddQueryString("query", query));
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public RoleDetailContract GetUserGroupDetail(int groupId)
        {
            try
            {
                var result = m_client.Get<RoleDetailContract>($"usergroup/{groupId}/detail");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void UpdateRole(int groupId, RoleContract data)
        {
            try
            {
                m_client.Put<object>($"role/{groupId}", data);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public int CreateRole(RoleContract request)
        {
            try
            {
                var result = m_client.Post<int>("role", request);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void DeleteRole(int groupId)
        {
            try
            {
                m_client.Delete($"role/{groupId}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public PermissionDataContract GetPermissionsForGroupAndBook(int groupId, long bookId)
        {
            try
            {
                var result = m_client.Get<PermissionDataContract>($"usergroup/{groupId}/book/{bookId}/permission");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void UpdateOrAddBooksToGroup(int groupId, long bookId, PermissionDataContract data)
        {
            try
            {
                m_client.Put<object>($"usergroup/{groupId}/book/{bookId}/permission", data);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void RemoveBooksFromGroup(int groupId, long bookId)
        {
            try
            {
                m_client.Delete($"usergroup/{groupId}/book/{bookId}/permission");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void AddSpecialPermissionsToRole(int groupId, IList<int> specialPermissionsIds)
        {
            try
            {
                m_client.Post<object>($"role/{groupId}/permission/special", new IntegerIdListContract
                {
                    IdList = specialPermissionsIds
                });
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void RemoveSpecialPermissionsFromRole(int groupId, IList<int> specialPermissionsIds)
        {
            try
            {
                m_client.Delete($"role/{groupId}/permission/special", new IntegerIdListContract
                {
                    IdList = specialPermissionsIds
                });
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }
    }
}
