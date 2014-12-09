using System.IO;
using System.ServiceModel;

namespace ITJakub.SearchService.Core.Exist
{
    [ServiceContract]
    public interface IExistClient
    {
        [OperationContract]
        [ExistAttribute(XqueryName = "get-page-list.xquery")]
        Stream GetPageList(string document);


        //[OperationContract]
        //[WebInvoke(Method = "POST", UriTemplate = "/db/apps/{projectName}/queries/get-page-list.xquery?document={documentId}&_xsl={xslPath}")]
        //string GetPageListWithTransformation(string projectName,string documentId, string xslPath);
    }
}