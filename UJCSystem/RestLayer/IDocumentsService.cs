using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Ujc.Naki.RestLayer
{
    [ServiceContract]
    public interface IDocumentsService
    {
        /// <summary>
        ///     Adds a new document resource to its collection. See API documentation.
        /// </summary>
        /// <param name="adv">document to be added in ADV format</param>
        /// <returns>response if document was created</returns>
        [OperationContract]
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Xml,
            UriTemplate = "/documents")]
        Stream CreateDocument(Stream adv);

        /// <summary>
        ///     Gets XML documents which satisfies specified query parameters. See API documentation.
        /// </summary>
        /// <returns>response with collection of documents</returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Xml,
            UriTemplate = "/documents",
            BodyStyle = WebMessageBodyStyle.Bare)]
        Stream GetDocuments();

        /// <summary>
        ///     Gets XML document under specified ID. If queried without query parameters, returns whole ADV (archive), otherwise part of document (XML). See API documentation.
        /// </summary>
        /// <param name="id">id of the document</param>
        /// <returns>response with part of XML document or whole ADV</returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            UriTemplate = "/documents/{id}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        Stream GetDocument(string id);

        /// <summary>
        ///     Gets XML document's metadata.
        /// </summary>
        /// <param name="id">id of the document</param>
        /// <returns>metadata of XML document</returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Xml,
            UriTemplate = "/documents/{id}/metadata",
            BodyStyle = WebMessageBodyStyle.Bare)]
        Stream GetDocumentMetadata(string id);
    }
}