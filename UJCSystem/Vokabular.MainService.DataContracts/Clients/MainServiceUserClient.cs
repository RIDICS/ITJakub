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
    public class MainServiceUserClient
    {
        private static readonly ILogger m_logger = ApplicationLogging.CreateLogger<MainServiceRestClient>();
        private readonly MainServiceRestClient m_client;

        public MainServiceUserClient(MainServiceRestClient client)
        {
            m_client = client;
        }

        public int CreateNewUser(CreateUserContract data)
        {
            try
            {
                //EnsureSecuredClient();
                var result = m_client.Post<int>("user", data);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public int CreateUserIfNotExist(int userExternalId)
        {
            try
            {
                var result = m_client.Post<int>("user/external", userExternalId);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public UserDetailContract GetCurrentUser()
        {
            try
            {
                return m_client.Get<UserDetailContract>("user/current");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void UpdateCurrentUser(UpdateUserContract data)
        {
            try
            {
                //EnsureSecuredClient();
                m_client.Put<object>("user/current", data);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void UpdateCurrentPassword(UpdateUserPasswordContract data)
        {
            try
            {
                //EnsureSecuredClient();
                m_client.Put<object>("user/current/password", data);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void UpdateCurrentUserContact(UpdateUserContactContract data)
        {
            try
            {
                m_client.Put<object>("user/current/contact", data);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public UserDetailContract GetUserDetail(int userId)
        {
            try
            {
                var result = m_client.Get<UserDetailContract>($"user/{userId}/detail");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public List<UserDetailContract> GetUserAutocomplete(string query)
        {
            try
            {
                var result = m_client.Get<List<UserDetailContract>>("user/autocomplete".AddQueryString("query", query));
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public PagedResultList<UserDetailContract> GetUserList(int start, int count, string query)
        {
            try
            {
                var url = "user".AddQueryString("start", start.ToString());
                url = url.AddQueryString("count", count.ToString());
                url = url.AddQueryString("filterByName", query);
                var result = m_client.GetPagedList<UserDetailContract>(url);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void UpdateUser(int userId, UpdateUserContract data)
        {
            try
            {
                m_client.Put<object>($"user/{userId}", data);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void UpdateUserContact(int userId, UpdateUserContactContract data)
        {
            try
            {
                m_client.Put<object>($"user/{userId}/contact", data);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public bool ConfirmUserContact(ConfirmUserContactContract data)
        {
            try
            {
                return m_client.Post<bool>($"user/current/contact/confirmation", data);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void ResendConfirmCode(UserContactContract data)
        {
            try
            {
                m_client.Post<object>($"user/current/contact/confirmation/resend", data);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void SetTwoFactor(UpdateTwoFactorContract data)
        {
            try
            {
                m_client.Put<object>($"user/current/two-factor", data);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void SelectTwoFactorProvider(UpdateTwoFactorProviderContract data)
        {
            try
            {
                m_client.Put<object>($"user/current/two-factor/provider", data);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public List<RoleContract> GetRolesByUser(int userId)
        {
            try
            {
                var result = m_client.Get<List<RoleContract>>($"user/{userId}/role");
                return result;
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
