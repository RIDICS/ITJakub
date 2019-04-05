using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Vokabular.OaiPmhImportManager.AutoMapperProfiles;
using Vokabular.ProjectImport.Model;

namespace Vokabular.OaiPmhImportManager
{
    public static class OaiPmhImportManagerContainerRegistration
    {
        public static void AddOaiPmhImportManager(this IServiceCollection services)
        {
            services.AddSingleton<Profile, MetadataFormatContractProfile>();
            services.AddSingleton<Profile, OaiPmhRepositoryInfoProfile>();
            services.AddSingleton<Profile, SetContractProfile>();

           services.AddSingleton<IProjectImportManager, OaiPmhProjectImportManager>();
        }
    }
}
