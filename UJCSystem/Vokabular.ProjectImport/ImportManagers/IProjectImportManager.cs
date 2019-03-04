using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Vokabular.DataEntities.Database.Entities;
using Project = Vokabular.ProjectParsing.Model.Entities.Project;
using ProjectImportMetadata = Vokabular.ProjectParsing.Model.Entities.ProjectImportMetadata;

namespace Vokabular.ProjectImport.ImportManagers
{
    public interface IProjectImportManager
    {
        Task ImportFromResource(ExternalResource resource, ITargetBlock<string> buffer, CancellationToken cancellationToken = default(CancellationToken));

        Project ImportRecord(ExternalResource resource, string id);

        ProjectImportMetadata ParseResponse(object response);

        string ExternalResourceTypeName { get; }
    }
}
