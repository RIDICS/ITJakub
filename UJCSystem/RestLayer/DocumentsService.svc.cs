using System;
using System.Diagnostics;
using System.IO;
using System.ServiceModel.Web;
using System.Text;
using AdvLib;
using Ujc.Naki.DataLayer;
using ServicesLayer;
using StreamsHelper = Ujc.Naki.DataLayer.StreamsHelper;

namespace Ujc.Naki.RestLayer
{
    public class DocumentsService : IDocumentsService
    {
        // rewriting URL, without .svc
        // http://www.robbagby.com/rest/rest-in-wcf-part-ix-controlling-the-uri/

        private const string LogPrefix = "DocumentsService: ";

        // DI in future?
        private readonly AdvFilesService m_advFilesSvc = new AdvFilesService();
        private readonly IXmlDbDao m_dao = new ExistDao();
        private readonly SearchService m_searchSvc = new SearchService();

        /// <see cref="IDocumentsService" />
        public Stream GetDocuments()
        {
            string qry = RestUtils.GetStringQp("qry");
            string title = RestUtils.GetStringQp("title");
            string author = RestUtils.GetStringQp("author");
            int datationFrom = RestUtils.GetIntQp("datation-from");
            int datationTo = RestUtils.GetIntQp("datation-to");
            DocumentKind? kind = EnumUtils.GetDocumentKind(RestUtils.GetStringQp("kind"));
            DocumentGenre? genre = EnumUtils.GetDocumentGenre(RestUtils.GetStringQp("genre"));
            DocumentOriginal? original = EnumUtils.GetDocumentOriginal(RestUtils.GetStringQp("original"));

            Debug.WriteLine(LogPrefix + "get documents[qry= " + qry + ", author=" + author + ", title=" + title +
                            ", datationFrom=" + datationFrom + ", datationTo=" + datationTo +
                            ", kind=" + kind + ", genre=" + genre + ", original=" + original + "]");

            string result = "<error>Server error</error>";
            if (ParameterExists(qry, title, author, kind, genre, original))
            {
                var criteria = new SearchCriteria(qry, title, author, datationFrom, datationTo, kind, genre, original);
                result = m_searchSvc.SearchDocuments(criteria);
            }

            return RestUtils.StringToStream(result);
        }

        /// <see cref="IDocumentsService" />
        public Stream CreateDocument(Stream adv)
        {
            byte[] advContent = StreamsHelper.StreamToByteArray(adv);
            AdvFile advFile = m_advFilesSvc.Unpack(advContent);
            string[] fileNames = advFile.GetFileNames();
            for (int i = 0; i < fileNames.Length; i++)
            {
                string name = fileNames[i];
                m_dao.Create(name, advFile.GetFile(name));
            }
            string response = "Document created";
            return new MemoryStream(Encoding.UTF8.GetBytes(response));
        }

        /// <see cref="IDocumentsService" />
        public Stream GetDocument(string strId)
        {
            int id = Int32.Parse(strId);
            Debug.WriteLine(LogPrefix + "get document[" + id + "]");

            string view = RestUtils.GetStringQp("view");

            if (view != null && view != string.Empty)
            {
                var docView = (DocumentView) EnumUtils.GetDocumentView(view);
                //string res = searchService.GetDocument(id, docView);

                if (docView == DocumentView.Imgpage || docView == DocumentView.Img)
                {
                    WebOperationContext.Current.OutgoingResponse.ContentType = "image/png";
                }
                else
                {
                    WebOperationContext.Current.OutgoingResponse.ContentType = "application/xml";
                }
                byte[] raw = m_searchSvc.GetView(id, docView);
                return new MemoryStream(raw);
            }
            AdvFile avd = m_searchSvc.GetFullDocument(id);
            Stream advStream = m_advFilesSvc.PackToStream(avd);
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/octet-stream";
            return advStream;
        }


        public Stream GetDocumentMetadata(string strId)
        {
            int id = Int32.Parse(strId);
            Debug.WriteLine(LogPrefix + "getMetadata[" + id + "]");

            WebOperationContext.Current.OutgoingResponse.ContentType = "application/xml";
            string metadata = m_searchSvc.GetDocumentMetadata(id);
            return new MemoryStream(Encoding.UTF8.GetBytes(metadata));
        }

        /// <summary>
        ///     Returns true if at least one of the arguments is not empty
        /// </summary>
        /// <param name="qry"></param>
        /// <param name="title"></param>
        /// <param name="author"></param>
        /// <param name="kind"></param>
        /// <param name="genre"></param>
        /// <param name="original"></param>
        /// <returns></returns>
        private bool ParameterExists(string qry, string title, string author, DocumentKind? kind, DocumentGenre? genre,
                                     DocumentOriginal? original)
        {
            if (qry != string.Empty || title != string.Empty || author != string.Empty || kind != null || genre != null ||
                original != null)
            {
                return true;
            }
            return false;
        }
    }
}