using ITJakub.Shared.Contracts;

namespace ITJakub.SearchService.Core.Exist
{
    public interface IExistResourceManager
    {
        string GetResourceUriTemplate(ResourceLevelEnumContract type);

        string GetQueryUriTemplate(string xqueryName, string queryStringParams);
    }
}