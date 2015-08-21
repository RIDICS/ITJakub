using System;
using System.Collections.Generic;
using System.IO;
using Castle.Components.DictionaryAdapter;
using ITJakub.BatchImport.Client.View;
using ITJakub.BatchImport.Client.ViewModel;

namespace ITJakub.BatchImport.Client.BusinessLogic
{
    public class FileUploadManager
    {

        private readonly List<FileModel> m_files = new List<FileModel>();



        public void AddFilesForUpload(Action<List<FileViewModel>, Exception> callback, string folderPath)
        {
            var result = new List<FileViewModel>();
            foreach (var file in Directory.GetFiles(folderPath))
            {
                m_files.Add(new FileModel(file));

                result.Add(new FileViewModel { FileName = file });
            }

            callback(result, null);
        }


        public void ProcessAllItems(Action<string, Exception> callback)
        {
            foreach (var item in m_files)
            {
                
            }
        }
    }

    public class FileModel
    {
        public string File { get; set; }

        public FileModel(string file)
        {
            File = file;
        }
    }
}