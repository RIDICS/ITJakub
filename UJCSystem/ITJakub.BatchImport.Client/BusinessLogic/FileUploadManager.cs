using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.Threading.Tasks;
using ITJakub.BatchImport.Client.BusinessLogic.Communication;
using ITJakub.BatchImport.Client.ViewModel;
using ITJakub.Shared.Contracts.Resources;
using Vokabular.MainService.DataContracts.Contracts;

namespace ITJakub.BatchImport.Client.BusinessLogic
{
    public class FileUploadManager
    {
        private readonly CommunicationProvider m_communicationProvider;
        private const string DefaultUploadMessage = "Uploaded by BatchImport client";        
        //private const string UserName = "test";
        //private const string Password = "PW:testtest";        

        public FileUploadManager(CommunicationProvider communicationProvider)
        {
            m_communicationProvider = communicationProvider;
        }


        private ConcurrentQueue<FileModel> m_files = new ConcurrentQueue<FileModel>();
        //private List<FileModel> m_files;                

        public void AddFilesForUpload(Action<List<FileViewModel>, Exception> callback, string folderPath)
        {
            m_files = new ConcurrentQueue<FileModel>();

            var result = new List<FileViewModel>();
            foreach (var file in Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories))
            {
                var fullPath = Path.Combine(folderPath, file);
                var fileName = Path.GetFileName(file);

                var fileModel = new FileModel(fullPath, fileName, FileStateType.Pending);
                m_files.Enqueue(fileModel);

                result.Add(new FileViewModel(fileModel));
            }

            callback(result, null);
        }


        public void ProcessAllItems(string username, string password, int threadCount, Action<string, Exception> callback)
        {            
            var formattedPassword = m_communicationProvider.GetFormattedPasswordAsAuthToken(password);
            ParallelLoopResult result = Parallel.ForEach(m_files,
                new ParallelOptions {MaxDegreeOfParallelism = threadCount},
                model => ProcessFile(model, username, formattedPassword, callback));
        }
    

        private void ProcessFile(FileModel file, string username, string password, Action<string, Exception> callback)
        {
            var session = Guid.NewGuid().ToString();

            //using (var client = m_communicationProvider.GetStreamingClientAuthenticated(username, password))
            using (var client = m_communicationProvider.GetMainServiceClient())
            {
                file.CurrentState = FileStateType.Uploading;
                using (var dataStream = GetDataStream(file.FullPath))
                {
                    try
                    {
                        var contract = new UploadResourceContract
                        {
                            FileName = file.FileName,
                            Data = dataStream,
                            SessionId = session
                        };

                        client.UploadResource(session, dataStream, file.FileName);
                    }
                    catch (Exception)
                    {
                        file.CurrentState = FileStateType.Error;
                        return;
                        //throw;
                    }
                }
            }

            file.CurrentState = FileStateType.Processing;

            //using (var client = m_communicationProvider.GetAuthenticatedClient(username, password))
            using (var client = m_communicationProvider.GetMainServiceClient())
            {
                try
                {
                    client.ProcessSessionAsImport(session, new NewBookImportContract
                    {
                        Comment = DefaultUploadMessage
                    });
                    file.CurrentState = FileStateType.Done;
                }
                catch (FaultException ex)
                {
                    file.ErrorMessage = ex.Message;
                    file.CurrentState = FileStateType.Error;
                    return;
                }
                catch (Exception)
                {
                    file.CurrentState = FileStateType.Error;
                    return;
                }                
            }

            callback(file.FullPath, null);
        }

        private Stream GetDataStream(string fullPath)
        {
            return new FileStream(fullPath, FileMode.Open);
        }     
    }

    public class FileModel
    {
        private FileStateType m_currentState;

        private string m_currentErrorMessage;

        public FileModel(string fullPath, string fileName, FileStateType currentState)
        {
            FullPath = fullPath;
            FileName = fileName;
            CurrentState = currentState;
        }

        public string FullPath { get; }

        public string FileName { get; }

        public string ErrorMessage
        {
            get { return m_currentErrorMessage; }
            set
            {
                m_currentErrorMessage = value;
                OnErrorMessageChanged(m_currentErrorMessage);
            }
        }

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

        public event EventHandler<string> ErrorMessageChanged;

        protected virtual void OnErrorMessageChanged(string e)
        {
            ErrorMessageChanged?.Invoke(this, e);
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