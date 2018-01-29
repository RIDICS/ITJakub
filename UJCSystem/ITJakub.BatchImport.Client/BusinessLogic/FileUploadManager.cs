using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ITJakub.BatchImport.Client.BusinessLogic.Communication;
using ITJakub.BatchImport.Client.ViewModel;
using Vokabular.MainService.DataContracts.Contracts;

namespace ITJakub.BatchImport.Client.BusinessLogic
{
    public class FileUploadManager
    {
        private readonly CommunicationProvider m_communicationProvider;
        private readonly AuthTokenStorage m_authTokenStorage;

        private const string DefaultUploadMessage = "Uploaded by BatchImport client";

        private ConcurrentQueue<FileModel> m_files = new ConcurrentQueue<FileModel>();

        public FileUploadManager(CommunicationProvider communicationProvider, AuthTokenStorage authTokenStorage)
        {
            m_communicationProvider = communicationProvider;
            m_authTokenStorage = authTokenStorage;
        }

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
            using (var client = m_communicationProvider.GetMainServiceClient())
            {
                try
                {
                    var signInResult = client.SignIn(new SignInContract
                    {
                        Username = username,
                        Password = password
                    });

                    m_authTokenStorage.AuthToken = signInResult.CommunicationToken;
                }
                catch (HttpRequestException exception)
                {
                    foreach (var fileModel in m_files)
                    {
                        fileModel.CurrentState = FileStateType.Error;
                        fileModel.ErrorMessage = exception.Message;
                    }
                    return;
                }
            }

            ParallelLoopResult result = Parallel.ForEach(m_files,
                new ParallelOptions {MaxDegreeOfParallelism = threadCount},
                model => ProcessFile(model, callback));
        }
    

        private void ProcessFile(FileModel file, Action<string, Exception> callback)
        {
            var session = Guid.NewGuid().ToString();

            using (var client = m_communicationProvider.GetMainServiceClient())
            {
                file.CurrentState = FileStateType.Uploading;
                using (var dataStream = GetDataStream(file.FullPath))
                {
                    try
                    {
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

            using (var client = m_communicationProvider.GetMainServiceClient()) // new client instance required because of specific client configuration
            {
                try
                {
                    client.ProcessSessionAsImport(session, new NewBookImportContract
                    {
                        Comment = DefaultUploadMessage
                    });
                    file.CurrentState = FileStateType.Done;
                }
                catch (HttpRequestException ex)
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