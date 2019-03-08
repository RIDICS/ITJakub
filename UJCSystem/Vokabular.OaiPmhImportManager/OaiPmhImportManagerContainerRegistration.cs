using Microsoft.Extensions.DependencyInjection;
using Vokabular.ProjectImport.ImportManagers;

namespace Vokabular.OaiPmhImportManager
{
    public static class OaiPmhImportManagerContainerRegistration
    {
        public static void AddOaiPmhImportManager(this IServiceCollection services)
        {
           services.AddSingleton<IProjectImportManager, OaiPmhProjectImportManager>();
        }
    }
}
