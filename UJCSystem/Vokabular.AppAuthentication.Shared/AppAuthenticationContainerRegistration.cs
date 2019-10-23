using Microsoft.Extensions.DependencyInjection;
using Vokabular.AppAuthentication.Shared.ServiceClient;
using Vokabular.MainService.DataContracts;

namespace Vokabular.AppAuthentication.Shared
{
    public static class AppAuthenticationContainerRegistration
    {
        public static void RegisterAppAuthenticationServices(this IServiceCollection services, MainServiceClientConfiguration mainServiceConfiguration)
        {
            services.AddSingleton<AuthenticationManager>();
            services.RegisterMainServiceClientComponents<MainServiceAuthTokenProvider, MainServiceClientLocalization>(mainServiceConfiguration);
        }
    }
}