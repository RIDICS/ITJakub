using System.IO;
using ITJakub.ITJakubService.DataContracts;

namespace ITJakub.ITJakubService.Core
{
    public class UploadManager
    {
        private readonly LocalFilesystemManager m_localFilesystemManager;

        public UploadManager(LocalFilesystemManager localFilesystemManager)
        {
            m_localFilesystemManager = localFilesystemManager;
        }

        public ProcessedFileInfoContract ProcessUploadedFile(Stream dataStream)
        {
            string tempName = m_localFilesystemManager.SaveToTempFolder(dataStream);
            throw new System.NotImplementedException();
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