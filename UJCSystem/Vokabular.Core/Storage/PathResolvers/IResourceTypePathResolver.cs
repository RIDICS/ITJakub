using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.Core.Storage.PathResolvers
{
    public interface IResourceTypePathResolver
    {
        ResourceType ResolvingResourceType { get; }

        bool PreserveFileNameInStorage { get; }

        string ResolvePath(long projectId, string bookVersionExternalId, string fileName);
    }
}