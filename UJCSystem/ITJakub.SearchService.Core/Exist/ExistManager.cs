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

        public async Task<List<BookPageContract>> GetPageList(string bookId,string versionId, string xslPath = null)
        {
            XDocument xmlDoc;
            using (Stream pageStream = await m_client.GetPageListAsync(bookId, versionId, xslPath))
            {
                xmlDoc = XDocument.Load(pageStream);
            }
            IEnumerable<XElement> pageBreakElements = xmlDoc.Root.Elements().Where(element => element.Name.LocalName == "pb");
            var pageList = new List<BookPageContract>();
            int position = 0;
            foreach (XElement pageBreakElement in pageBreakElements)
            {
                ++position;
                pageList.Add(new BookPageContract
                {
                    Position = position,
                    Text = pageBreakElement.Attribute("n").Value,
                    XmlId = pageBreakElement.Attribute(pageBreakElement.GetNamespaceOfPrefix("xml") + "id").Value
                });
            }

            return pageList;
        }

        public async Task UploadBookFile(string bookId, string fileName, Stream dataStream)
        {
            await m_client.UploadBookFileAsync(bookId, fileName, dataStream);
        }

        public async Task UploadVersionFile(string bookId, string versionId, string fileName, Stream filStream)
        {
            await m_client.UploadVersionFileAsync(bookId, versionId, fileName, filStream);
        }

        public async Task UploadSharedFile(string fileName, Stream filStream)
        {
            await m_client.UploadSharedFileAsync(fileName, filStream);
        }

        public async Task<string> GetPageByPositionFromStart(string bookId, string versionId, int pagePosition, string transformationName, ResourceLevelEnumContract transformationLevel)
        {
            string xslPath = m_existResourceManager.GetTransformationUri(transformationName, transformationLevel, bookId, versionId);
            return await m_client.GetPageByPositionFromStart(bookId, versionId, pagePosition, xslPath);
        }

        public async Task<string> GetPageByName(string bookId, string versionId, string pageName, string transformationName, ResourceLevelEnumContract transformationLevel)
        {
            string xslPath = m_existResourceManager.GetTransformationUri(transformationName, transformationLevel, bookId, versionId);
            return await m_client.GetPageByName(bookId, versionId, pageName, xslPath);
        }

        public async Task<string> GetPagesByName(string bookId, string versionId, string start, string end, string transformationName, ResourceLevelEnumContract transformationLevel)
        {
            string xslPath = m_existResourceManager.GetTransformationUri(transformationName, transformationLevel, bookId, versionId);
            return await m_client.GetPagesByName(bookId, versionId, start, end, xslPath);
        }

        public async Task<string> GetPageByXmlIdAsync(string bookId, string versionId, string xmlId, string transformationName, ResourceLevelEnumContract transformationLevel)
        {
            string xslPath = m_existResourceManager.GetTransformationUri(transformationName, transformationLevel, bookId, versionId);
            return await m_client.GetPageByXmlIdAsync(bookId, versionId, xmlId, xslPath);
        }
    }
}