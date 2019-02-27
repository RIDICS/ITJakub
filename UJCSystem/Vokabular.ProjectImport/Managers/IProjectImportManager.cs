using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Vokabular.ProjectImport.DataEntities.Database;
using Vokabular.ProjectParsing.Model.Entities;

namespace Vokabular.ProjectImport.Managers
{
    public interface IProjectImportManager
    {
        Task ImportFromResource(Resource resource, ITargetBlock<string> buffer, CancellationToken cancellationToken = default(CancellationToken));

        Project ImportRecord(Resource resource, string id);

        ResourceType ResourceType { get; }
    }
}
