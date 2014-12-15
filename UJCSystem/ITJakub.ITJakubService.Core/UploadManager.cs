using System;
using ITJakub.FileStorage;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.Shared.Contracts.Resources;

namespace ITJakub.ITJakubService.Core
{
    public class UploadManager
    {
        private readonly FileSystemManager m_filesystemManager;

        public UploadManager(FileSystemManager filesystemManager)
        {
            m_filesystemManager = filesystemManager;
        }

        public ProcessedFileInfoContract ProcessUploadedFile(UploadResourceContract uploadResourceContract)
        {
            throw new NotImplementedException();
        }

        private ProcessedFileInfoContract GetFileInfoFromTempFile(string fileName)
        {
            throw new NotImplementedException();
        }
    }
}