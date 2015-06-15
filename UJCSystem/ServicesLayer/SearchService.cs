using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Diagnostics;
using Ujc.Naki.DataLayer;
using AdvLib;

namespace ServicesLayer
{
    [ServiceContract]
    public interface ISearchService
    {
        /// <summary>
        /// Searches all documents by specified parameters.
        /// </summary>
        /// <param name="criteria">search criteria</param>
        /// <returns>XML with documents which satisfy the search</returns>
        [OperationContract]
        string SearchDocuments(SearchCriteria criteria);

        /// <summary>
        /// Gets a document in its XML form specified by its ID, view and page.
        /// </summary>
        /// <param name="id">id of the document</param>
        /// <param name="view">view of the document</param>
        /// <param name="page">page - typically used for image page views</param>
        /// <returns>XML result</returns>
        [OperationContract]
        byte[] GetView(int id, DocumentView view, int page);

        /// <summary>
        /// Gets a document in its XML form specified by its ID and view.
        /// </summary>
        /// <param name="id">id of the document</param>
        /// <param name="view">view of the document</param>
        /// <returns>XML result</returns>
        [OperationContract]
        byte[] GetView(int id, DocumentView view);

        /// <summary>
        /// Gets metadata of document specified by its ID.
        /// </summary>
        /// <param name="id">ID of the document</param>
        /// <returns>metadata about document</returns>
        [OperationContract]
        string GetDocumentMetadata(int id);

        /// <summary>
        /// Gets full document (ADV file format) with specified ID.
        /// </summary>
        /// <param name="id">id of the document</param>
        /// <returns>ADV archive of document</returns>s
        [OperationContract]
        AdvFile GetFullDocument(int id);
    }

    public class SearchService : ISearchService
    {
        //  private static readonly string NO_RESULTS = "<no-result />";

        private const string XmlHeader = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";

        private static readonly IXmlDbDao m_dao = new ExistDao();
        private readonly IXmlFormatDescriptor m_formatDesc = new TeiP5Descriptor();
        private readonly IFilenamesResolver m_filenames = new DefaultFilenamesResolver(m_dao);

        /// <summary>
        /// Searches all documents by specified parameters.
        /// </summary>
        /// <param name="criteria">search criteria</param>
        /// <returns>XML with documents which satisfy the search</returns>
        public string SearchDocuments(SearchCriteria criteria)
        {
            XQBuilder queryBuilder = new XQBuilder(m_formatDesc, m_filenames);
            queryBuilder.SetCollectionName(m_dao.GetViewsCollection());

            if (criteria.Qry != string.Empty) { queryBuilder.AddSearch(criteria.Qry); }
            if (criteria.Title != string.Empty) { queryBuilder.AddTitleWhere(criteria.Title); }
            if (criteria.Author != string.Empty) { queryBuilder.AddAuthorWhere(criteria.Author); }
            if (criteria.DatationFrom != -1) { queryBuilder.AddDatationFromWhere(criteria.DatationFrom); }
            if (criteria.DatationTo != -1) { queryBuilder.AddDatationToWhere(criteria.DatationTo); }
            if (criteria.Kind != null) { queryBuilder.AddKindWhere((DocumentKind)criteria.Kind); }
            if (criteria.Genre != null) { queryBuilder.AddGenreWhere((DocumentGenre)criteria.Genre); }
            if (criteria.Original != null) { queryBuilder.AddOriginalWhere((DocumentOriginal)criteria.Original); }

            string qry = queryBuilder.Build();
            string xml = m_dao.QueryXml(qry);
            return XmlHeader + xml;
        }

        /// <summary>
        /// Gets a document in its XML form specified by its ID, view and page.
        /// </summary>
        /// <param name="id">id of the document</param>
        /// <param name="view">view of the document</param>
        /// <param name="page">page - typically used for image page views</param>
        /// <returns>XML result</returns>
        public byte[] GetView(int id, DocumentView view, int page)
        {
            string name = "";
            if (view == DocumentView.Img || view == DocumentView.Imgpage)
            {
                name = m_filenames.ConstructImgName(id, view, page);
            }
            else
            {
                name = m_filenames.ConstructXmlName(id, view);
            }
            return m_dao.Read(name);
        }

        /// <summary>
        /// Gets a document in its XML form specified by its ID and view.
        /// </summary>
        /// <param name="id">id of the document</param>
        /// <param name="view">view of the document</param>
        /// <returns>XML result</returns>
        public byte[] GetView(int id, DocumentView view)
        {
            return GetView(id, view, 0);
        }

        /// <summary>
        /// Gets metadata of document specified by its ID.
        /// </summary>
        /// <param name="id">ID of the document</param>
        /// <returns>metadata about document</returns>
        public string GetDocumentMetadata(int id)
        {
            XQBuilder queryBuilder = new XQBuilder(m_formatDesc, m_filenames);
            queryBuilder.SetCollectionName(m_dao.GetViewsCollection());
            queryBuilder.SetForMetadataRetrieval(id);
            string qry = queryBuilder.Build();
            string xml = m_dao.QueryXml(qry);
            return XmlHeader + xml;
        }

        /// <summary>
        /// Gets full document (ADV file format) with specified ID.
        /// </summary>
        /// <param name="id">id of the document</param>
        /// <returns>ADV archive of document</returns>s
        public AdvFile GetFullDocument(int id)
        {
            AdvFile avdFile = new AdvFile();

            string[] result = m_filenames.GetViewsFromID(id);
            foreach (string viewStr in result)
            {
                DocumentView view = (DocumentView)EnumUtils.GetDocumentView(viewStr);
                string filename = "";
                if (view == DocumentView.Imgpage || view == DocumentView.Img)
                {
                    filename = m_filenames.ConstructImgName(id, view);
                }
                else
                {
                    filename = m_filenames.ConstructXmlName(id, view);
                }
                byte[] content = GetView(id, view);
                avdFile.PutFile(filename, content);

                // append meta file
                byte[] metaFile = Encoding.UTF8.GetBytes(GetDocumentMetadata(id));
                avdFile.PutMetaFile(metaFile);
            }

            return avdFile;
        }
    }
}
