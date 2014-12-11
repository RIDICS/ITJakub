using System;
using System.IO;
using System.Xml.Linq;
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
            string tempName = m_filesystemManager.SaveTempFile(uploadResourceContract.Data);
            return new ProcessedFileInfoContract {FileGuid = "ABCDEFG", VersionId = "XYZ"};
                //TODO just for testing purposes
            //TODO call program for converting docx to xml here
            ProcessedFileInfoContract fileInfo = GetFileInfoFromTempFile(tempName);
            m_filesystemManager.RenameTempFile(tempName, fileInfo.FileGuid);
            return fileInfo;
        }

        private ProcessedFileInfoContract GetFileInfoFromTempFile(string fileName)
        {
            throw new NotImplementedException();
        }
    }
}