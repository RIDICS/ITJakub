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

        public ExistManager(ExistClient existClient)
        {
            m_client = existClient;
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
                pageList.Add(new BookPageContract {Position = position, Text = pageBreakElement.Attribute("n").Value});
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

        public async Task<string> GetPageByPositionFromStart(string bookId, string versionId, int pagePosition)
        {
            return await m_client.GetPageByPositionFromStart(bookId, versionId, pagePosition);
        }

        public async Task<string> GetPageByName(string bookId, string versionId, string pageName)
        {
            return await m_client.GetPageByName(bookId, versionId, pageName);
        }

        public async Task<string> GetPagesByName(string bookId, string versionId, string start, string end)
        {
            return await m_client.GetPagesByName(bookId, versionId, start, end);
        }
    }
}