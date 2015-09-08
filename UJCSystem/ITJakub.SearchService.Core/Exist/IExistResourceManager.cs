using ITJakub.Shared.Contracts;

namespace ITJakub.SearchService.Core.Exist
{
    public interface IExistResourceManager
    {
        string GetResourceUriTemplate(ResourceLevelEnumContract type);

        string GetQueryUriWithParams(string xqueryName, string queryStringParams);

        string GetQueryUri(string xqueryName);

        string GetTransformationUri(string transformationName, OutputFormatEnumContract outputFormat, 
					ResourceLevelEnumContract transformationLevel, string bookGuid, string bookVersion);
    }
}