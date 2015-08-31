using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ITJakub.BatchImport.Client.ViewModel;
using ITJakub.ITJakubService.DataContracts.Clients;
using ITJakub.Shared.Contracts.Resources;

namespace ITJakub.BatchImport.Client.BusinessLogic
{
    public class FileUploadManager
    {
        private const string DefaultUploadMessage = "Uploaded by BatchImport client";
        private List<FileModel> m_files;


        public void AddFilesForUpload(Action<List<FileViewModel>, Exception> callback, string folderPath)
        {
            m_files = new List<FileModel>();

            var result = new List<FileViewModel>();
            foreach (var file in Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories))
            {
                var fullPath = Path.Combine(folderPath, file);
                var fileName = Path.GetFileName(file);

                var fileModel = new FileModel(fullPath, fileName, FileStateType.Pending);
                m_files.Add(fileModel);

                result.Add(new FileViewModel(fileModel));
            }

            callback(result, null);
        }


        public void ProcessAllItems(Action<string, Exception> callback)
        {
            var allTasks = new List<Task>();
            foreach (var file in m_files)
            {
                allTasks.Add(Task.Factory.StartNew(() => ProcessFile(file)));
            }

            Task.WaitAll(allTasks.ToArray());
        }

        private void ProcessFile(FileModel file)
        {
            var session = Guid.NewGuid().ToString();
            using (var client = new ItJakubServiceStreamedClient())
            {
                file.CurrentState = FileStateType.Uploading;
                using (var dataStream = GetDataStream(file.FullPath))
                {
                    var contract = new UploadResourceContract
                    {
                        FileName = file.FileName,
                        Data = dataStream,
                        SessionId = session
                    };

                    client.AddResource(contract);
                }
            }

            file.CurrentState = FileStateType.Processing;
                        
            using (var client = new ItJakubServiceClient())
            {
                try
                {
                    client.ProcessSession(session, DefaultUploadMessage);
                }
                catch (Exception)
                {
                    file.CurrentState = FileStateType.Error;
                    throw;
                }

                file.CurrentState = FileStateType.Done;
            }
        }

        private Stream GetDataStream(string fullPath)
        {
            return new FileStream(fullPath, FileMode.Open);
        }
    }

    public class FileModel
    {
        private FileStateType m_currentState;

        public FileModel(string fullPath, string fileName, FileStateType currentState)
        {
            FullPath = fullPath;
            FileName = fileName;
            CurrentState = currentState;
        }

        public string FullPath { get; }

        public string FileName { get; }

        public FileStateType CurrentState
        {
            get { return m_currentState; }
            set
            {
                m_currentState = value;
                OnStateChanged(m_currentState);
            }
        }

        public event EventHandler<FileStateType> StateChanged;

        protected virtual void OnStateChanged(FileStateType e)
        {
            StateChanged?.Invoke(this, e);
        }
    }

    public enum FileStateType
    {
        Pending,
        Uploading,
        Processing,
        Done,
        Error
    }
}