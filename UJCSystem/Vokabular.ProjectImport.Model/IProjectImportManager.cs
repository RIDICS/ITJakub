using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Project = Vokabular.ProjectParsing.Model.Entities.Project;
using ProjectImportMetadata = Vokabular.ProjectParsing.Model.Entities.ProjectImportMetadata;

namespace Vokabular.ProjectImport.Model
{
    public interface IProjectImportManager
    {
        string ExternalRepositoryTypeName { get; }

        Task ImportFromResource(string repository, ITargetBlock<object> buffer, CancellationToken cancellationToken = default(CancellationToken));

        Project ImportRecord(string repository, string id);

        ProjectImportMetadata ParseResponse(object response);
    }
}
