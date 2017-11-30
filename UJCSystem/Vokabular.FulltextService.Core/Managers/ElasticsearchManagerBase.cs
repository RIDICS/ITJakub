using Nest;
using Vokabular.FulltextService.Core.Communication;

namespace Vokabular.FulltextService.Core.Managers
{
    public abstract class ElasticsearchManagerBase
    {
        protected readonly CommunicationProvider CommunicationProvider;
        protected const string SnapshotIndex = "snapshotindex"; 
        protected const string PageIndex = "pageindex"; 
        protected const string PageType = "page";
        protected const string SnapshotType = "snapshot";
        protected const string SnapshotIdField = "snapshotId";
        protected const string ProjectIdField = "projectId";
        protected const string PageTextField = "pageText";
        protected const string SnapshotTextField = "snapshotText";
        protected const string IdField = "_id";
        
        protected ElasticsearchManagerBase(CommunicationProvider communicationProvider)
        {
            CommunicationProvider = communicationProvider;
        }
    }
}