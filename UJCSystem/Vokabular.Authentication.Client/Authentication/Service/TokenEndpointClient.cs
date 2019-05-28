using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Vokabular.Authentication.Client.Authentication.Options;
using Vokabular.Authentication.Client.Client;

namespace Vokabular.Authentication.Client.Authentication.Service
{
    public class TokenEndpointClient : ITokenEndpointClient
    {
        private readonly AutomaticTokenManagementOptions m_managementOptions;
        private readonly IOptionsSnapshot<OpenIdConnectOptions> m_oidcOptions;
        private readonly IAuthenticationSchemeProvider m_schemeProvider;
        private readonly TokenEndpointHttpClientProvider m_tokenEndpointHttpClientProvider;

        public TokenEndpointClient(
            IOptions<AutomaticTokenManagementOptions> managementOptions,
            IOptionsSnapshot<OpenIdConnectOptions> oidcOptions,
            IAuthenticationSchemeProvider schemeProvider,
            TokenEndpointHttpClientProvider tokenEndpointHttpClientProvider)
        {
            m_managementOptions = managementOptions.Value;
            m_oidcOptions = oidcOptions;
            m_schemeProvider = schemeProvider;
            m_tokenEndpointHttpClientProvider = tokenEndpointHttpClientProvider;
        }

        public async Task<TokenResponse> GetAccessTokenAsync(string scope)
        {
            var oidcOptions = await GetOidcOptionsAsync();
            var configuration = await oidcOptions.ConfigurationManager.GetConfigurationAsync(default);

            var httpClient = GetClientAsync(oidcOptions);

            return await httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = configuration.TokenEndpoint,
                ClientId = oidcOptions.ClientId,
                ClientSecret = oidcOptions.ClientSecret,
                Scope = scope
            });
        }

        public async Task<TokenResponse> RefreshTokenAsync(string refreshToken)
        {
            var oidcOptions = await GetOidcOptionsAsync();
            var configuration = await oidcOptions.ConfigurationManager.GetConfigurationAsync(default);

            var httpClient = GetClientAsync(oidcOptions);

            return await httpClient.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = configuration.TokenEndpoint,
                ClientId = oidcOptions.ClientId,
                ClientSecret = oidcOptions.ClientSecret,
                RefreshToken = refreshToken
            });
        }

        public async Task<TokenRevocationResponse> RevokeTokenAsync(string refreshToken)
        {
            var oidcOptions = await GetOidcOptionsAsync();
            var configuration = await oidcOptions.ConfigurationManager.GetConfigurationAsync(default);

            var httpClient = GetClientAsync(oidcOptions);

            return await httpClient.RevokeTokenAsync(new TokenRevocationRequest
            {
                Address = configuration.AdditionalData[OidcConstants.Discovery.RevocationEndpoint].ToString(),
                ClientId = oidcOptions.ClientId,
                ClientSecret = oidcOptions.ClientSecret,
                Token = refreshToken,
                TokenTypeHint = OidcConstants.TokenTypes.RefreshToken
            });
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

        private System.Net.Http.HttpClient GetClientAsync(OpenIdConnectOptions oidcOptions)
        {
            return m_tokenEndpointHttpClientProvider.GetClientAsync(oidcOptions);
        }
    }
}