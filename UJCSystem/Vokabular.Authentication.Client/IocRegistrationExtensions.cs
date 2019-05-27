using Microsoft.Extensions.DependencyInjection;
using Vokabular.Authentication.Client.Client;
using Vokabular.Authentication.Client.Client.Auth;
using Vokabular.Authentication.Client.Configuration;
using Vokabular.Authentication.Client.Provider;
using Vokabular.Authentication.Client.Storage;

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
            services.AddScoped<AuthApiAccessTokenProvider>();

            services.AddSingleton<ITokenStorage, InMemoryTokenStorage>();

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
    }
}
