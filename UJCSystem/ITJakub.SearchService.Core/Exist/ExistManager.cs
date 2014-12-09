using System.IO;

namespace ITJakub.SearchService.Core.Exist
{
    public class ExistManager
    {
        private readonly ExistClient m_client;

        public ExistManager(ExistClient existClient)
        {
            m_client = existClient;
        }

        public Stream GetPageList(string documentId, string xslPath)
        {
            return m_client.GetPageList(documentId,xslPath);
        }

        public Stream GetPageList(string documentId)
        {
            return m_client.GetPageList(documentId);
        }

        public string GetPageByPositionFromStart(string documentId, int pagePosition)
        {
            throw new System.NotImplementedException();
        }

        public string GetPageByName(string documentId, string pageName)
        {
            throw new System.NotImplementedException();
        }

        public string GetPagesByName(string documentId, string start, string end)
        {
            throw new System.NotImplementedException();
        }
    }
}