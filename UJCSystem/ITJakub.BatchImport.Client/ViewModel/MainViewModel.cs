using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.BatchImport.Client.DataService;

namespace ITJakub.BatchImport.Client.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataService m_dataService;
        private string m_folderPath;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {
            m_dataService = dataService;

            InitializeData();
            InitializeCommands();
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
        }

        private void InitializeCommands()
        {
            ConvertCommand = new RelayCommand(ConvertSelectedPath);            
        }

        private void ConvertSelectedPath()
        {
            //m_dataService.            
        }

        public string FolderPath
        {
            get { return m_folderPath; }
            set { m_folderPath = value; RaisePropertyChanged();}
        }

        public RelayCommand ConvertCommand { get; set; }

        private void InitializeData()
        {
            m_dataService.TestMethod((result, error) =>
            {
                if (error != null)
                {
                    return;
                }
                if (result != null)
                {
                    FolderPath = result;
                }
            });
        }


    }
}