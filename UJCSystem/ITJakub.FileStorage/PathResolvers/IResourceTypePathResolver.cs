using ITJakub.FileStorage.Resources;

namespace ITJakub.FileStorage.PathResolvers
{
    public interface IResourceTypePathResolver
    {
        ResourceTypeEnum ResolvingResourceType();

        string ResolvePath(string bookId, string bookVersionId, string fileName);
    }
}