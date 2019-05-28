using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Vokabular.Authentication.Client.Authentication.Options;
using Vokabular.Authentication.Client.Client;

namespace Vokabular.Authentication.Client.Authentication.Events
{
    /// <summary>
    /// This class takes care about refreshing access tokens on each request and revoking refresh tokens on sing out, for more info see: IN2-2314
    /// </summary>
    public class AutomaticTokenManagementCookieEvents : CookieAuthenticationEvents
    {
        private const string ExpiresAtTokenName = "expires_at";

        private readonly ITokenEndpointClient m_client;
        private readonly AutomaticTokenManagementOptions m_options;

        private static readonly ConcurrentDictionary<string, bool> m_pendingRefreshTokenRequests =
            new ConcurrentDictionary<string, bool>();

        public AutomaticTokenManagementCookieEvents(
            ITokenEndpointClient client,
            IOptions<AutomaticTokenManagementOptions> options)
        {
            m_client = client;
            m_options = options.Value;
        }

        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            var tokens = context.Properties.GetTokens().ToList();
            if (!tokens.Any())
            {
                return;
            }

            var refreshToken = tokens.SingleOrDefault(t => t.Name == OpenIdConnectParameterNames.RefreshToken);
            if (refreshToken == null)
            {
                return;
            }

            var expiresAt = tokens.SingleOrDefault(t => t.Name == ExpiresAtTokenName);
            if (expiresAt == null)
            {
                return;
            }

            var dtExpires = DateTimeOffset.Parse(expiresAt.Value, CultureInfo.InvariantCulture);
            var dtRefresh = dtExpires.Subtract(m_options.RefreshBeforeExpiration);

            if (dtRefresh < DateTime.UtcNow)
            {
                var shouldRefresh = m_pendingRefreshTokenRequests.TryAdd(refreshToken.Value, true);
                if (shouldRefresh)
                {
                    try
                    {
                        var response = await m_client.RefreshTokenAsync(refreshToken.Value);

                        if (response.IsError)
                        {
                            return;
                        }

                        context.Properties.UpdateTokenValue(OpenIdConnectParameterNames.AccessToken, response.AccessToken);
                        context.Properties.UpdateTokenValue(OpenIdConnectParameterNames.RefreshToken, response.RefreshToken);

                        var newExpiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(response.ExpiresIn);
                        context.Properties.UpdateTokenValue(ExpiresAtTokenName, newExpiresAt.ToString("o", CultureInfo.InvariantCulture));

                        await context.HttpContext.SignInAsync(context.Principal, context.Properties);
                    }
                    finally
                    {
                        m_pendingRefreshTokenRequests.TryRemove(refreshToken.Value, out _);
                    }
                }
            }
        }

        public override async Task SigningOut(CookieSigningOutContext context)
        {
            if (m_options.RevokeRefreshTokenOnSignout == false) return;

            var result = await context.HttpContext.AuthenticateAsync();

            if (!result.Succeeded)
            {
                return;
            }

            var tokens = result.Properties.GetTokens().ToList();
            if (!tokens.Any())
            {
                return;
            }

            var refreshToken = tokens.SingleOrDefault(t => t.Name == OpenIdConnectParameterNames.RefreshToken);
            if (refreshToken == null)
            {
                return;
            }

            var response = await m_client.RevokeTokenAsync(refreshToken.Value);
        }
    }
}