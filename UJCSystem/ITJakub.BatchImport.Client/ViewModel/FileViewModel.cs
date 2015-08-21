using GalaSoft.MvvmLight;

namespace ITJakub.BatchImport.Client.ViewModel
{
    public class FileViewModel : ViewModelBase
    {
        private string m_fileName;

        public string FileName
        {
            get { return m_fileName; }
            set { m_fileName = value; RaisePropertyChanged(); }
        }

    }
}