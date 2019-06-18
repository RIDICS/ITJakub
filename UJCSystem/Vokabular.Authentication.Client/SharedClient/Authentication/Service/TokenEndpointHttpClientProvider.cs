using System;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace Vokabular.Authentication.Client.SharedClient.Authentication.Service
{
    public class TokenEndpointHttpClientProvider : IDisposable
    {
        private readonly ConcurrentDictionary<string, System.Net.Http.HttpClient> m_httpClients = new ConcurrentDictionary<string, System.Net.Http.HttpClient>();

        public System.Net.Http.HttpClient GetClientAsync(OpenIdConnectOptions oidcOptions)
        {
            var httpClient = m_httpClients.GetOrAdd(oidcOptions.Authority, baseUrl => new System.Net.Http.HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            });

            return httpClient;
        }

        public void Dispose()
        {
            foreach (var httpClient in m_httpClients)
            {
                httpClient.Value.Dispose();
            }
        }
    }
}