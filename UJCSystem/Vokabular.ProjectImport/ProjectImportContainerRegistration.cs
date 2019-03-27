using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Vokabular.Marc21ProjectParser;
using Vokabular.OaiPmhImportManager;
using Vokabular.ProjectImport.AutoMapperProfiles;
using Vokabular.ProjectImport.ImportPipeline;
using Vokabular.ProjectImport.Managers;

namespace Vokabular.ProjectImport
{
    public static class ProjectImportContainerRegistration
    {
        public static void AddProjectImportServices(this IServiceCollection container)
        {
            container.AddSingleton<Profile, RepositoryImportProgressProfile>();

            container.AddScoped<ExternalRepositoryManager>();
            container.AddScoped<FilteringExpressionSetManager>();
            container.AddScoped<FilteringManager>();
            container.AddScoped<ImportHistoryManager>();
            container.AddScoped<ImportMetadataManager>();
            container.AddScoped<ProjectManager>();
            
            container.AddHostedService<ProjectImportHostedService>();
            container.AddSingleton<MainImportManager>();
            container.AddSingleton<ImportManager>();

            container.AddScoped<ImportPipelineBuilder>();

            container.AddMarc21ProjectParsingServices();
            container.AddOaiPmhImportManager();
        }
    }
}
