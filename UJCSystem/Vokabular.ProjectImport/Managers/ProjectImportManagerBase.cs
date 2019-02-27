using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Vokabular.ProjectImport.DataEntities.Database;
using Vokabular.ProjectParsing.Model.Entities;

namespace Vokabular.ProjectImport.Managers
{
    public abstract class ProjectImportManagerBase : IProjectImportManager
    {
        public ResourceType ResourceType { get; }

        protected ProjectImportManagerBase(ResourceType resourceType)
        {
            ResourceType = resourceType;
        }

        public abstract Task ImportFromResource(Resource resource, ITargetBlock<string> buffer, CancellationToken cancellationToken = default(CancellationToken));
        public abstract Project ImportRecord(Resource resource, string id);
    }
}
