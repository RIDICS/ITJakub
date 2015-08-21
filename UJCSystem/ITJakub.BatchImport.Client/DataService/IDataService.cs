using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITJakub.BatchImport.Client.View;
using ITJakub.BatchImport.Client.ViewModel;

namespace ITJakub.BatchImport.Client.DataService
{
    public interface IDataService
    {
        void TestMethod(Action<string, Exception> callback);
        void LoadAllItems(Action<List<FileViewModel>, Exception> callback, string folderPath);
        void ProcessItems(Action<string,Exception> callback);
    }

    public class DesignDataService : IDataService
    {
        public void TestMethod(Action<string, Exception> callback)
        {
            callback("design test path", null);
        }

        public void LoadAllItems(Action<List<FileViewModel>, Exception> callback, string folderPath)
        {
            callback(new List<FileViewModel>
            {
                new FileViewModel {FileName = "TestovaciFile.docx"}
            }, null);
        }

        public void ProcessItems(Action<string, Exception> callback)
        {            
        }
    }

}
