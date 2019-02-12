using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace ITJakub.Web.Hub.Authentication
{
    public class TokenEndpointService
    {
        private readonly AutomaticTokenManagementOptions m_managementOptions;
        private readonly IOptionsSnapshot<OpenIdConnectOptions> m_oidcOptions;
        private readonly IAuthenticationSchemeProvider m_schemeProvider;

        public TokenEndpointService(
            IOptions<AutomaticTokenManagementOptions> managementOptions,
            IOptionsSnapshot<OpenIdConnectOptions> oidcOptions,
            IAuthenticationSchemeProvider schemeProvider)
        {
            m_managementOptions = managementOptions.Value;
            m_oidcOptions = oidcOptions;
            m_schemeProvider = schemeProvider;
        }

        public async Task<TokenResponse> RefreshTokenAsync(string refreshToken)
        {
            var oidcOptions = await GetOidcOptionsAsync();
            var configuration = await oidcOptions.ConfigurationManager.GetConfigurationAsync(CancellationToken.None);

            var data = new Dictionary<string, string>
            {
                {OidcConstants.RefreshToken, refreshToken},
                {OidcConstants.GrantType, OidcConstants.RefreshToken},
                {OidcConstants.ClientSecret, oidcOptions.ClientSecret},
                {OidcConstants.ClientId, oidcOptions.ClientId}
            };

            var response = await PostData(configuration.TokenEndpoint, data);
            return new TokenResponse(ParseResponse(await response.Content.ReadAsStringAsync(), response.Content.Headers.ContentType));
        }

        public async void RevokeTokenAsync(string refreshToken)
        {
            var oidcOptions = await GetOidcOptionsAsync();
            var configuration = await oidcOptions.ConfigurationManager.GetConfigurationAsync(CancellationToken.None);
            var revocationEndpoint = configuration.AdditionalData["revocation_endpoint"].ToString();

            var data = new Dictionary<string, string>
            {
                {OidcConstants.Token, refreshToken},
                {OidcConstants.TokenTypeHint, OidcConstants.RefreshToken},
                {OidcConstants.ClientSecret, oidcOptions.ClientSecret},
                {OidcConstants.ClientId, oidcOptions.ClientId}
            };

            await PostData(revocationEndpoint, data);
        }

        private async Task<HttpResponseMessage> PostData(string address, IDictionary<string, string> data)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, address);
            httpRequest.Headers.Accept.Clear();
            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencode"));
           
            httpRequest.Content = new FormUrlEncodedContent(data);

            using (var client = new HttpClient())
            {
                var response = await client.SendAsync(httpRequest).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                return response;
            }
        }

        private async Task<OpenIdConnectOptions> GetOidcOptionsAsync()
        {
            if (string.IsNullOrEmpty(m_managementOptions.Scheme))
            {
                var scheme = await m_schemeProvider.GetDefaultChallengeSchemeAsync();
                return m_oidcOptions.Get(scheme.Name);
            }
            else
            {
                return m_oidcOptions.Get(m_managementOptions.Scheme);
            }
        }

        private JObject ParseResponse(string userInfoResponse, MediaTypeHeaderValue contentType)
        {
            if (contentType.MediaType.Equals("application/json", StringComparison.OrdinalIgnoreCase))
            {
                return JObject.Parse(userInfoResponse);
            }

            if (contentType.MediaType.Equals("application/jwt", StringComparison.OrdinalIgnoreCase))
            {
                return JObject.FromObject(new JwtSecurityToken(userInfoResponse).Payload);
            }

            throw new NotSupportedException($"Unknown response type: {contentType.MediaType}");
        }
    }
}