using System.IO;
using System.Xml.Linq;
using ITJakub.FileStorage;
using ITJakub.ITJakubService.DataContracts;

namespace ITJakub.ITJakubService.Core
{
    public class UploadManager
    {
        private readonly FilesystemManager m_filesystemManager;
        private readonly XmlProcessingManager m_xmlProcessingManager;

        public UploadManager(FilesystemManager filesystemManager, XmlProcessingManager xmlProcessingManager)
        {
            m_filesystemManager = filesystemManager;
            m_xmlProcessingManager = xmlProcessingManager;
        }

        public ProcessedFileInfoContract ProcessUploadedFile(UploadFileContract uploadFileContract)
        {
            string tempName = m_filesystemManager.SaveTempFile(uploadFileContract.Data);
            return new ProcessedFileInfoContract {FileGuid = "ABCDEFG", VersionId = "XYZ"};
                //TODO just for testing purposes
            //TODO call program for converting docx to xml here
            ProcessedFileInfoContract fileInfo = GetFileInfoFromTempFile(tempName);
            m_filesystemManager.RenameTempFile(tempName, fileInfo.FileGuid);
            return fileInfo;
        }

        private ProcessedFileInfoContract GetFileInfoFromTempFile(string fileName)
        {
            using (FileStream fileStream = m_filesystemManager.OpenTempFile(fileName))
            {
                XDocument fileHeader = m_xmlProcessingManager.ParseHeader(fileStream);
                return m_xmlProcessingManager.GetInfoFromHeader(fileHeader);
            }
        }

        public void SaveFrontImageForFile(UploadImageContract uploadImageContract)
        {
            m_filesystemManager.SaveFrontImage(uploadImageContract.FileGuid, uploadImageContract.Name,
                uploadImageContract.Data);
        }

        public void SavePageImageForFile(UploadImageContract uploadImageContract)
        {
            m_filesystemManager.SaveImage(uploadImageContract.FileGuid, uploadImageContract.Name,
                uploadImageContract.Data);
        }
    }
}