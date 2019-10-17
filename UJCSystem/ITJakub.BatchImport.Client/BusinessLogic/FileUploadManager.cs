﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading.Tasks;
using ITJakub.BatchImport.Client.BusinessLogic.Communication;
using ITJakub.BatchImport.Client.ViewModel;
using Vokabular.MainService.DataContracts.Contracts;

namespace ITJakub.BatchImport.Client.BusinessLogic
{
    public class FileUploadManager
    {
        private readonly CommunicationProvider m_communicationProvider;
        private readonly AuthManager m_authenticationManager;

        private const string DefaultUploadMessage = "Uploaded by BatchImport client";

        private ConcurrentQueue<FileModel> m_files = new ConcurrentQueue<FileModel>();

        public FileUploadManager(CommunicationProvider communicationProvider, AuthManager authenticationManager)
        {
            m_communicationProvider = communicationProvider;
            m_authenticationManager = authenticationManager;
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


        public void ProcessAllItems(int threadCount, Action<string, Exception> callback)
        {
            try
            {
                m_authenticationManager.SignInAsync().Wait();

                ParallelLoopResult result = Parallel.ForEach(m_files,
                    new ParallelOptions {MaxDegreeOfParallelism = threadCount},
                    model => ProcessFile(model, callback));
            }
            catch (AggregateException e)
            {
                if (e.GetBaseException().GetType() == typeof(AuthenticationException))
                {
                    foreach (var fileModel in m_files)
                    {
                        fileModel.CurrentState = FileStateType.Error;
                        fileModel.ErrorMessage = e.GetBaseException().Message;
                    }
                }
                else
                {
                    throw;
                }
            }
        }


        private void ProcessFile(FileModel file, Action<string, Exception> callback)
        {
            var session = Guid.NewGuid().ToString();

            var client = m_communicationProvider.GetMainServiceSessionClient();
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

            file.CurrentState = FileStateType.Processing;

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