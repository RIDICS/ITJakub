using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.Core.Storage.PathResolvers
{
    public interface IResourceTypePathResolver
    {
        ResourceType ResolvingResourceType();

        string ResolvePath(long projectId, string bookVersionExternalId, string fileName);
    }
}