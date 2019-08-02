using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Vokabular.Marc21ProjectParser;
using Vokabular.OaiPmhImportManager;
using Vokabular.ProjectImport.AutoMapperProfiles;
using Vokabular.ProjectImport.ImportPipeline;
using Vokabular.ProjectImport.Managers;
using Vokabular.ProjectImport.Permissions;

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
            container.AddScoped<ImportedProjectMetadataManager>();
            container.AddScoped<ImportedRecordMetadataManager>();
            container.AddScoped<ImportedProjectManager>();

            container.AddScoped<CommunicationFactory>();

            container.AddScoped<IPermissionsProvider, PermissionProvider>();
            
            container.AddHostedService<ProjectImportBackgroundService>();
            container.AddSingleton<MainImportManager>();
            container.AddSingleton<ImportManager>();

            container.AddScoped<ImportPipelineBuilder>();
            container.AddScoped<ImportPipelineDirector>();
            container.AddScoped<ImportPipelineManager>();

            container.AddMarc21ProjectParsingServices();
            container.AddOaiPmhImportManager();
        }
    }
}
