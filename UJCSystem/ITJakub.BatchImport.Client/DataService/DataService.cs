using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public void LoadAllItems(Action<List<FileViewModel>, Exception> callback, string folderPath)
        {
            Task.Factory.StartNew(() => m_fileUploadManager.AddFilesForUpload(callback, folderPath));
        }

        public void ProcessItems(int threadCount, Action<string, Exception> callback)
        {
            Task.Factory.StartNew(() => m_fileUploadManager.ProcessAllItems(threadCount, callback));
        }
    }
}