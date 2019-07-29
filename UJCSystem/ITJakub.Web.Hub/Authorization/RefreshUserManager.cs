using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace ITJakub.Web.Hub.Authorization
{
    public class RefreshUserManager
    {
        public async Task RefreshUserClaimsAsync(HttpContext httpContext)
        {
            if (!httpContext.User.Identity.IsAuthenticated)
            {
                return;
            }

            var authenticateResult = await httpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (authenticateResult == null || !authenticateResult.Succeeded)
            {
                throw new AuthenticationException("Can not re-authenticate user");
            }

            var properties = authenticateResult.Properties;

            var authenticationService = (AuthenticationService)httpContext.RequestServices.GetRequiredService<IAuthenticationService>();
            var openIdConnectHandler = (OpenIdConnectHandler)await authenticationService.Handlers.GetHandlerAsync(
                httpContext,
                OpenIdConnectDefaults.AuthenticationScheme
            );

            var configurationAsync = await openIdConnectHandler.Options.ConfigurationManager.GetConfigurationAsync(CancellationToken.None);
            var userInfoEndpoint = configurationAsync.UserInfoEndpoint;

            var accessToken = properties.Items[".Token.access_token"];

            if (!string.IsNullOrEmpty(accessToken))
            {
                var responseMessage = await openIdConnectHandler.Options.Backchannel.SendAsync(
                    new HttpRequestMessage(HttpMethod.Get, userInfoEndpoint)
                    {
                        Headers =
                        {
                            Authorization = new AuthenticationHeaderValue("Bearer", accessToken)
                        }
                    }
                );
                responseMessage.EnsureSuccessStatusCode();

                var userInfoResponse = await responseMessage.Content.ReadAsStringAsync();
                var contentType = responseMessage.Content.Headers.ContentType;
                var jUser = ParseResponse(userInfoResponse, contentType);

                var primaryIdentity = authenticateResult.Principal.Identity;
                var identity = authenticateResult.Principal.Identities.FirstOrDefault(x =>
                    x.Name == primaryIdentity.Name && x.AuthenticationType == primaryIdentity.AuthenticationType);

                if (identity == null)
                {
                    throw new AuthenticationException("Can not refresh primary identity");
                }

                foreach (var claim in identity.Claims.ToList())
                {
                    switch (claim.Type)
                    {
                        case "sid":
                        case ClaimTypes.NameIdentifier:
                        case "http://schemas.microsoft.com/identity/claims/identityprovider":
                        case "http://schemas.microsoft.com/claims/authnmethodsreferences":
                            continue;
                    }

                    identity.RemoveClaim(claim);
                }

                foreach (var claimAction in openIdConnectHandler.Options.ClaimActions)
                {
                    claimAction.Run(jUser, identity, openIdConnectHandler.Scheme.Name);
                }
            }

            await httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                authenticateResult.Principal,
                authenticateResult.Properties
            );
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