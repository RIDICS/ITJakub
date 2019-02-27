using Microsoft.Extensions.DependencyInjection;
using Vokabular.CommunicationService;
using Vokabular.Marc21ProjectParser;
using Vokabular.ProjectImport.Managers;

namespace Vokabular.ProjectImport
{
    public static class ProjectImportContainerRegistration
    {
        public static void AddProjectImportServices(this IServiceCollection container)
        {
            container.AddHostedService<ProjectImportHostedService>();
            container.AddSingleton<MainImportManager>();

            container.AddSingleton<ImportManager>();
            
            container.AddSingleton<IProjectImportManager, OaiPmhProjectImportManager>();

            container.AddMarc21ProjectParsingServices();
            container.AddCommunicationServices();
        }
    }
}
