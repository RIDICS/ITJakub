using Microsoft.Extensions.DependencyInjection;
using Vokabular.Marc21ProjectParser;
using Vokabular.OaiPmhImportManager;
using Vokabular.ProjectImport.Managers;

namespace Vokabular.ProjectImport
{
    public static class ProjectImportContainerRegistration
    {
        public static void AddProjectImportServices(this IServiceCollection container)
        {
            container.AddScoped<ExternalRepositoryManager>();
            container.AddScoped<FilteringExpressionSetManager>();
            container.AddScoped<FilteringManager>();
            container.AddScoped<ImportHistoryManager>();
            container.AddScoped<ImportMetadataManager>();
            container.AddScoped<ProjectManager>();
            
            container.AddHostedService<ProjectImportHostedService>();
            container.AddSingleton<MainImportManager>();
            container.AddSingleton<ImportManager>();

            container.AddMarc21ProjectParsingServices();
            container.AddOaiPmhImportManager();
        }
    }
}
