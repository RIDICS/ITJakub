using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Vokabular.Shared.AspNetCore.Container.Extensions
{
    public static class MapperConfigExtensions
    {
        public static void AddAutoMapper(this IServiceCollection services)
        {
            services.AddSingleton<IConfigurationProvider>(provider => new MapperConfiguration(cfg =>
            {
                var profiles = provider.GetServices<Profile>();

                foreach (var profile in profiles)
                {
                    cfg.AddProfile(profile);
                }
            }));
            
            services.AddSingleton<IMapper>(provider => new Mapper(provider.GetRequiredService<IConfigurationProvider>()));
        }
    }
}
