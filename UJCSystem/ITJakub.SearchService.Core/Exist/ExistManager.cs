using System.Collections.Generic;
using System.IO;
using ITJakub.SearchService.Core.Exist.XmlParser;
using ITJakub.Shared.Contracts;

namespace ITJakub.SearchService.Core.Exist
{
    public class ExistManager
    {
        private readonly ExistClient m_client;
        private readonly IExistResourceManager m_existResourceManager;
        private readonly XmlParserManager m_xmlParserManager;

        public ExistManager(ExistClient existClient, IExistResourceManager existResourceManager, XmlParserManager xmlParserManager)
        {
            m_client = existClient;
            m_existResourceManager = existResourceManager;
            m_xmlParserManager = xmlParserManager;
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

        public string GetPageByPositionFromStart(string bookId, string versionId, int pagePosition,
            string transformationName, ResourceLevelEnumContract transformationLevel)
        {
            var xslPath = m_existResourceManager.GetTransformationUri(transformationName, transformationLevel, bookId,
                versionId);
            return m_client.GetPageByPositionFromStart(bookId, versionId, pagePosition, xslPath);
        }

        public string GetPageByName(string bookId, string versionId, string pageName, string transformationName,
            ResourceLevelEnumContract transformationLevel)
        {
            var xslPath = m_existResourceManager.GetTransformationUri(transformationName, transformationLevel, bookId,
                versionId);
            return m_client.GetPageByName(bookId, versionId, pageName, xslPath);
        }

        public string GetPagesByName(string bookId, string versionId, string start, string end,
            string transformationName, ResourceLevelEnumContract transformationLevel)
        {
            var xslPath = m_existResourceManager.GetTransformationUri(transformationName, transformationLevel, bookId,
                versionId);
            return m_client.GetPagesByName(bookId, versionId, start, end, xslPath);
        }
    }
}