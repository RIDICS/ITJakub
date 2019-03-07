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
        Task ImportFromResource(ExternalRepository repository, ITargetBlock<string> buffer, CancellationToken cancellationToken = default(CancellationToken));

        Project ImportRecord(ExternalRepository repository, string id);

        ProjectImportMetadata ParseResponse(object response);

        string ExternalRepositoryTypeName { get; }
    }
}
