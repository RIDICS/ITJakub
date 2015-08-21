using System;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using ITJakub.BatchImport.Client.DataService;

namespace ITJakub.BatchImport.Client.ViewModel
{
    /// <summary>
    ///     This class contains properties that the main View can data bind to.
    ///     <para>
    ///         Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    ///     </para>
    ///     <para>
    ///         You can also use Blend to data bind with the tool's support.
    ///     </para>
    ///     <para>
    ///         See http://www.galasoft.ch/mvvm
    ///     </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataService m_dataService;
        private string m_folderPath;

        /// <summary>
        ///     Initializes a new instance of the MainViewModel class.
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

        public string FolderPath
        {
            get { return m_folderPath; }
            set
            {
                m_folderPath = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand ConvertCommand { get; set; }
        public RelayCommand LoadItemsCommand { get; set; }

        public ObservableCollection<FileViewModel> FileItems { get; set; }        


        private void InitializeCommands()
        {
            ConvertCommand = new RelayCommand(ConvertSelectedPath);
            LoadItemsCommand = new RelayCommand(LoadItems);
        }

        public void LoadItems()
        {
            var item = FolderPath;
            m_dataService.LoadAllItems((result, error) =>
            {
                if (error != null)
                    return;
                if (result != null)
                {
                    foreach (var variable in result)
                    {
                        DispatcherHelper.CheckBeginInvokeOnUI(()=>FileItems.Add(variable));
                    }
                }
                
            }, FolderPath);


            item = item.ToLowerInvariant();
        }

        private void ConvertSelectedPath()
        {
            m_dataService.ProcessItems((resultProcessed, error) =>
            {
                if (error != null)
                    return;
                if (resultProcessed != null)
                {
                    foreach (var variable in FileItems.ToList())
                    {
                        if (variable.FileName == resultProcessed)
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(() => FileItems.Remove(variable));
                            break;
                        }
                            
                    }
                }
            });
        }

        private void InitializeData()
        {
            FileItems = new ObservableCollection<FileViewModel>();

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