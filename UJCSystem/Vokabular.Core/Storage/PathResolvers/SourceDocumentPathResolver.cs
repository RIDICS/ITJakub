using System.IO;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.Core.Storage.PathResolvers
{
    public class SourceDocumentPathResolver : IResourceTypePathResolver
    {
        public ResourceType ResolvingResourceType => ResourceType.SourceDocument;

        public bool PreserveFileNameInStorage => true;

        public string ResolvePath(long projectId, string bookVersionExternalId, string fileName)
        {
            return Path.Combine(projectId.ToString(), bookVersionExternalId, fileName);
        }
    }
}