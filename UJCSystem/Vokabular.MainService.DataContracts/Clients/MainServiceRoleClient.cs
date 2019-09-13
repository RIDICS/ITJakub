using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Permission;
using Vokabular.RestClient.Extensions;
using Vokabular.RestClient.Results;
using Vokabular.Shared;
using Vokabular.Shared.Extensions;

namespace Vokabular.MainService.DataContracts.Clients
{
    public class MainServiceRoleClient
    {
        private static readonly ILogger m_logger = ApplicationLogging.CreateLogger<MainServiceRestClient>();
        private readonly MainServiceRestClient m_client;

        public MainServiceRoleClient(MainServiceRestClient client)
        {
            m_client = client;
        }

        public PagedResultList<UserContract> GetUsersByRole(int roleId, int start, int count, string query)
        {
            try
            {
                var url = $"role/{roleId}/user".AddQueryString("start", start.ToString());
                url = url.AddQueryString("count", count.ToString());
                url = url.AddQueryString("filterByName", query);
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

        public void AddUserToRole(int userId, int roleId)
        {
            try
            {
                m_client.Post<object>($"role/{roleId}/user/{userId}", null);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void RemoveUserFromRole(int userId, int roleId)
        {
            try
            {
                m_client.Delete($"role/{roleId}/user/{userId}");
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

        public RoleDetailContract GetRoleDetail(int roleId)
        {
            try
            {
                var result = m_client.Get<RoleDetailContract>($"role/{roleId}/detail");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void UpdateRole(int roleId, RoleContract data)
        {
            try
            {
                m_client.Put<object>($"role/{roleId}", data);
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

        public void DeleteRole(int roleId)
        {
            try
            {
                m_client.Delete($"role/{roleId}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void AddBooksToRole(int roleId, IList<long> bookIds)
        {
            try
            {
                m_client.Post<object>($"role/{roleId}/permission/book", new AddBookToRoleRequestContract
                {
                    BookIdList = bookIds
                });
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void RemoveBooksFromRole(int roleId, IList<long> bookIds)
        {
            try
            {
                m_client.Delete($"role/{roleId}/permission/book", new AddBookToRoleRequestContract
                {
                    BookIdList = bookIds
                });
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public List<PermissionContract> GetPermissionsForRole(int roleId)
        {
            try
            {
                var result = m_client.Get<List<PermissionContract>>($"role/{roleId}/permission");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void AddSpecialPermissionsToRole(int roleId, IList<int> specialPermissionsIds)
        {
            try
            {
                m_client.Post<object>($"role/{roleId}/permission/special", new IntegerIdListContract
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

        public void RemoveSpecialPermissionsFromRole(int roleId, IList<int> specialPermissionsIds)
        {
            try
            {
                m_client.Delete($"role/{roleId}/permission/special", new IntegerIdListContract
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
