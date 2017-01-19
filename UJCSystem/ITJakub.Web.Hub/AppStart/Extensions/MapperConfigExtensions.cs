using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ITJakub.Web.Hub.Extensions
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
    }
}
