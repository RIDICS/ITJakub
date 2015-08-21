using System;
using System.Collections.Generic;
using System.IO;
using ITJakub.BatchImport.Client.BusinessLogic;
using ITJakub.BatchImport.Client.ViewModel;

namespace ITJakub.BatchImport.Client.DataService
{
    public class DataService : IDataService
    {
        private readonly FileUploadManager m_fileUploadManager;

        public DataService(FileUploadManager fileUploadManager)
        {
            m_fileUploadManager = fileUploadManager;
        }

        public void TestMethod(Action<string, Exception> callback)
        {
            callback("test", null);
        }

        public void LoadAllItems(Action<List<FileViewModel>, Exception> callback, string folderPath)
        {
           m_fileUploadManager.AddFilesForUpload(callback,folderPath);
        
        }

        public void ProcessItems(Action<string, Exception> callback)
        {
            m_fileUploadManager.ProcessAllItems(callback);
        }
    }
}