using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Vokabular.Shared.AspNetCore.Container.Extensions
{
    public static class MapperConfigExtensions
    {
        public static void ConfigureAutoMapper(this IApplicationBuilder applicationBuilder)
        {
            var profiles = applicationBuilder.ApplicationServices.GetServices<Profile>();

            Mapper.Initialize(cfg =>
            {
                foreach (var profile in profiles)
                {
                    cfg.AddProfile(profile);
                }
            });
        }

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
