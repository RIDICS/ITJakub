using ITJakub.SearchService.Core.Exist.Attributes;

namespace ITJakub.SearchService.Core.Exist
{
    public interface IExistResourceManager
    {
        string GetResourceUriTemplate(ResourceLevelType type);

        string GetQueryUriTemplate(string xqueryName, string queryStringParams);
    }
}