using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Vokabular.Authentication.TicketStore.Store;

namespace Vokabular.Authentication.TicketStore
{
    public static class AuthenticationStoreServicesExtension
    {
        public static IServiceCollection SetAuthenticationTicketStore<TTicketStore>(this IServiceCollection services, CacheTicketStoreConfig config)
            where TTicketStore : class, ITicketStore
        {
            services.AddSingleton<ITicketStore, TTicketStore>();
            services.AddSingleton<IPostConfigureOptions<CookieAuthenticationOptions>, PostConfigureCookieAuthenticationOptions>();

            if (config != null)
            {
                services.AddSingleton(config);
            }

            return services;
        }
    }
}