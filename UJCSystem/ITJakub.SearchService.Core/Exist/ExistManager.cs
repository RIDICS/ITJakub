using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using ITJakub.Shared.Contracts;

namespace ITJakub.SearchService.Core.Exist
{
    public class ExistManager
    {
        private readonly ExistClient m_client;
        private readonly IExistResourceManager m_existResourceManager;

        public ExistManager(ExistClient existClient, IExistResourceManager existResourceManager)
        {
            m_client = existClient;
            m_existResourceManager = existResourceManager;
        }

        public List<BookPageContract> GetPageList(string bookId,string versionId, string xslPath = null)
        {
            XDocument xmlDoc;
            using (Stream pageStream = m_client.GetPageList(bookId, versionId, xslPath))
            {
                xmlDoc = XDocument.Load(pageStream);
            }
            IEnumerable<XElement> pageBreakElements = xmlDoc.Root.Elements().Where(element => element.Name.LocalName == "pb");
            var pageList = new List<BookPageContract>();
            int position = 0;
            foreach (XElement pageBreakElement in pageBreakElements)
            {
                ++position;
                pageList.Add(new BookPageContract {Position = position, Text = pageBreakElement.Attribute("n").Value});
            }

            return pageList;
        }

        public void UploadBookFile(string bookId, string fileName, Stream dataStream)
        {
            m_client.UploadBookFile(bookId, fileName, dataStream);
        }

        public void UploadVersionFile(string bookId, string versionId, string fileName, Stream filStream)
        {
            m_client.UploadVersionFile(bookId, versionId, fileName, filStream);
        }

        public void UploadSharedFile(string fileName, Stream filStream)
        {
            m_client.UploadSharedFile(fileName, filStream);
        }

        public string GetPageByPositionFromStart(string bookId, string versionId, int pagePosition, string transformationName, ResourceLevelEnumContract transformationLevel)
        {
            string xslPath = m_existResourceManager.GetTransformationUri(transformationName, transformationLevel, bookId, versionId);
            return m_client.GetPageByPositionFromStart(bookId, versionId, pagePosition, xslPath);
        }

        public string GetPageByName(string bookId, string versionId, string pageName, string transformationName, ResourceLevelEnumContract transformationLevel)
        {
            string xslPath = m_existResourceManager.GetTransformationUri(transformationName, transformationLevel, bookId, versionId);
            return m_client.GetPageByName(bookId, versionId, pageName, xslPath);
        }

        public string GetPagesByName(string bookId, string versionId, string start, string end, string transformationName, ResourceLevelEnumContract transformationLevel)
        {
            string xslPath = m_existResourceManager.GetTransformationUri(transformationName, transformationLevel, bookId, versionId);
            return m_client.GetPagesByName(bookId, versionId, start, end, xslPath);
        }
    }
}