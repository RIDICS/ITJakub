using System;
using GalaSoft.MvvmLight;
using ITJakub.BatchImport.Client.BusinessLogic;

namespace ITJakub.BatchImport.Client.ViewModel
{
    public class FileViewModel : ViewModelBase, IDisposable
    {
        private readonly FileModel m_model;
        private bool m_disposed;


        private string m_fileName;
        private FileStateType m_state;
        private string m_fullPath;

        public FileViewModel(FileModel model)
        {
            m_model = model;

            FileName = m_model.FileName;
            FullPath = m_model.FullPath;
            State = m_model.CurrentState;

            SubscribeToEvents();
        }

        public string FullPath
        {
            get { return m_fullPath; }
            set { m_fullPath = value; RaisePropertyChanged();}
        }

        public FileStateType State
        {
            get { return m_state; }
            set { m_state = value; RaisePropertyChanged();}
        }

        public string FileName
        {
            get { return m_fileName; }
            set
            {
                m_fileName = value;
                RaisePropertyChanged();
            }
        }

        private void SubscribeToEvents()
        {
            m_model.StateChanged += StateChanged;
        }

        private void UnsubscribeFromEvents()
        {
            m_model.StateChanged -= StateChanged;
        }

        private void StateChanged(object sender, FileStateType e)
        {
            State = e;
        }




        #region IDisposable Implementation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                    UnsubscribeFromEvents();
                }

                // Call the appropriate methods to clean up 
                // unmanaged resources here. 

                // Note disposing has been done.
                m_disposed = true;
            }
        }

        ~FileViewModel()
        {
            // Do not re-create Dispose clean-up code here. 
            // Calling Dispose(false) is optimal in terms of 
            // readability and maintainability.
            Dispose(false);
        }

        #endregion
    }
}