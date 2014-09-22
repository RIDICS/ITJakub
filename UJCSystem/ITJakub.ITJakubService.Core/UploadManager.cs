using System;
using System.IO;
using System.Xml;
using ITJakub.ITJakubService.DataContracts;

namespace ITJakub.ITJakubService.Core
{
    public class UploadManager
    {
        private readonly LocalFilesystemManager m_localFilesystemManager;
        private readonly XmlProcessingManager m_xmlProcessingManager;

        public UploadManager(LocalFilesystemManager localFilesystemManager, XmlProcessingManager xmlProcessingManager)
        {
            m_localFilesystemManager = localFilesystemManager;
            m_xmlProcessingManager = xmlProcessingManager;
        }

        public ProcessedFileInfoContract ProcessUploadedFile(Stream dataStream)
        {
            string tempName = m_localFilesystemManager.SaveTempFile(dataStream);
            ProcessedFileInfoContract fileInfo = GetFileInfoFromTempFile(tempName);
            m_localFilesystemManager.RenameTempFile(tempName, fileInfo.Guid);
            return fileInfo;
        }

        private ProcessedFileInfoContract GetFileInfoFromTempFile(string fileName)
        {
            using (var fileStream = m_localFilesystemManager.OpenTempFile(fileName))
            {
                var fileHeader = m_xmlProcessingManager.ParseHeader(fileStream);
                return m_xmlProcessingManager.GetInfoFromHeader(fileHeader);
            }
        }

        public void SaveFrontImageForFile(Stream dataStream)
        {
            throw new System.NotImplementedException();
        }

        public void SaveImagesForFile(Stream dataStream)
        {
            throw new System.NotImplementedException();
        }
    }
}