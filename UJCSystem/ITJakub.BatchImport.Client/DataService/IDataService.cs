using System;
using System.Collections.Generic;
using ITJakub.BatchImport.Client.BusinessLogic;
using ITJakub.BatchImport.Client.ViewModel;

namespace ITJakub.BatchImport.Client.DataService
{
    public interface IDataService
    {
        

        void LoadAllItems(Action<List<FileViewModel>, Exception> callback, string folderPath);

        void ProcessItems(Action<string, Exception> callback);
    }

    public class DesignDataService : IDataService
    {     

        public void LoadAllItems(Action<List<FileViewModel>, Exception> callback, string folderPath)
        {
            callback(new List<FileViewModel>
            {
                new FileViewModel(new FileModel("C:/FullPath", "C:/FileName", FileStateType.Pending))
            }, null);
        }

        public void ProcessItems(Action<string, Exception> callback)
        {
        }
    }
}