using ITJakub.Core.Resources;
using ITJakub.Shared.Contracts.Resources;

namespace ITJakub.Core.PathResolvers
{
    public interface IResourceTypePathResolver
    {
        ResourceType ResolvingResourceType();

        string ResolvePath(string bookId, string bookVersionId, string fileName);
    }
}