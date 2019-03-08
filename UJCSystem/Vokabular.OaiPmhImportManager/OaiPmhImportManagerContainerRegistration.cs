using Microsoft.Extensions.DependencyInjection;
using Vokabular.ProjectImport.Model;

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
