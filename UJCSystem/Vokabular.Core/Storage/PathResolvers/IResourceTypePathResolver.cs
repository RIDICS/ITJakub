using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.Core.Storage.PathResolvers
{
    public interface IResourceTypePathResolver
    {
        ResourceType ResolvingResourceType();

        string ResolvePath(string bookId, string bookVersionId, string fileName);
    }
}