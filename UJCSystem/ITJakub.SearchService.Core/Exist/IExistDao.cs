using System.ServiceModel;
using System.ServiceModel.Web;

namespace ITJakub.SearchService.Core.Exist
{
    [ServiceContract]
    public interface IExistDao
    {

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/db/apps/{projectName}/queries/get-page-list.xquery?document={documentId}", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Xml)]
        string GetPageList(string projectName, string documentId);


        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/db/apps/{projectName}/queries/get-page-list.xquery?document={documentId}&_xsl={xslPath}")]
        string GetPageListWithTransformation(string projectName,string documentId, string xslPath);
    }
}