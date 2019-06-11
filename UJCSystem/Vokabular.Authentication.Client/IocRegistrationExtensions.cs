using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Vokabular.Authentication.Client.Client;
using Vokabular.Authentication.Client.Client.Auth;
using Vokabular.Authentication.Client.Configuration;
using Vokabular.Authentication.Client.Provider;
using Vokabular.Authentication.Client.SharedClient.Authentication.Events;
using Vokabular.Authentication.Client.SharedClient.Authentication.Options;
using Vokabular.Authentication.Client.SharedClient.Authentication.Service;
using Vokabular.Authentication.Client.SharedClient.Client;
using Vokabular.Authentication.Client.SharedClient.Config;
using Vokabular.Authentication.Client.SharedClient.Provider;
using Vokabular.Authentication.Client.SharedClient.Storage;

namespace Vokabular.Authentication.Client
{
    public static class IocRegistrationExtensions
    {
        public static void RegisterAuthorizationHttpClientComponents<TClientLocalization>(this IServiceCollection services,
            AuthServiceCommunicationConfiguration configuration = null,
            OpenIdConnectConfig openIdConfiguration = null,
            AuthServiceControllerBasePathsConfiguration pathConfiguration = null)
            where TClientLocalization : class, IAuthorizationServiceClientLocalization
        {
            services.AddScoped<IAuthorizationServiceClientLocalization, TClientLocalization>();

            services.AddScoped<AuthorizationServiceHttpClient>();

            services.AddScoped<AuthServiceControllerBasePathsProvider>();

            services.AddScoped<RegistrationApiClient>();
            services.AddScoped<ExternalIdentityProviderApiClient>();
            services.AddScoped<FileResourceApiClient>();
            services.AddScoped<NonceApiClient>();
            services.AddScoped<UserApiClient>();
            services.AddScoped<RoleApiClient>();
            services.AddScoped<PermissionApiClient>();
            services.AddScoped<ContactApiClient>();
            services.TryAddScoped<AuthApiAccessTokenProvider>();

            services.TryAddSingleton<ITokenStorage, InMemoryTokenStorage>();

            services.TryAddTransient<ITokenEndpointClient, TokenEndpointClient>();
            services.TryAddSingleton<TokenEndpointHttpClientProvider>();

            if (configuration != null)
            {
                services.AddSingleton(configuration);
            }

            if (openIdConfiguration != null)
            {
                services.AddSingleton(openIdConfiguration);
            }

            if (pathConfiguration != null)
            {
                services.AddSingleton(pathConfiguration);
            }
        }

        public static void RegisterAutomaticTokenManagement(this IServiceCollection services)
        {
            services.AddTransient<AutomaticTokenManagementCookieEvents>();
            services.AddSingleton<IConfigureOptions<CookieAuthenticationOptions>, AutomaticTokenManagementConfigureCookieOptions>();
        }
    }
}
