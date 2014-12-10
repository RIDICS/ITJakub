namespace ITJakub.SearchService.Core.Exist
{
    public interface IExistResourceManager
    {
        string GetResourceUriTemplate();

        string GetQueryUriTemplate(string xqueryName, string queryStringParams);
    }
}