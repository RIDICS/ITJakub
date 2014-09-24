using System;
using System.IO;
using System.Xml.Linq;
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

        public ProcessedFileInfoContract ProcessUploadedFile(UploadFileContract uploadFileContract)
        {
            string tempName = m_localFilesystemManager.SaveTempFile(uploadFileContract.Data);
            ProcessedFileInfoContract fileInfo = GetFileInfoFromTempFile(tempName);
            m_localFilesystemManager.RenameTempFile(tempName, fileInfo.Guid);
            return fileInfo;
        }

        private ProcessedFileInfoContract GetFileInfoFromTempFile(string fileName)
        {
            using (FileStream fileStream = m_localFilesystemManager.OpenTempFile(fileName))
            {
                XDocument fileHeader = m_xmlProcessingManager.ParseHeader(fileStream);
                return m_xmlProcessingManager.GetInfoFromHeader(fileHeader);
            }
        }

        public void SaveFrontImageForFile(UploadImageContract uploadImageContract)
        {
            m_localFilesystemManager.SaveFrontImage(uploadImageContract.FileGuid, uploadImageContract.Name, uploadImageContract.Data);
        }

        public void SavePageImageForFile(UploadImageContract uploadImageContract)
        {
            m_localFilesystemManager.SaveImage(uploadImageContract.FileGuid, uploadImageContract.Name, uploadImageContract.Data);
        }
    }
}