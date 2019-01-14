using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Vokabular.Authentication.DataContracts.User;
using Vokabular.RestClient;
using Vokabular.Shared;
using Vokabular.Shared.Extensions;

namespace Vokabular.Authentication.Client
{
    public class AuthenticationClient : FullRestClientBase
    {
        private static readonly ILogger m_logger = ApplicationLogging.CreateLogger<AuthenticationClient>();

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

        public UserContract CreateUser(CreateUserContract contract)
        {
            try
            {
                var result = Post<UserContract>("api/v1/registration/create", contract);
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
                var result = Get<UserContract>("api/v1/user/" + id);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }
    }
}