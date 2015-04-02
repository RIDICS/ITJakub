using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.SampleApp.Service;
using ITJakub.MobileApps.Client.Shared.ViewModel;

namespace ITJakub.MobileApps.Client.SampleApp.ViewModel
{
    public class SampleEditorViewModel : EditorBaseViewModel
    {
        private readonly SampleDataService m_dataService;
        private string m_test;

        public SampleEditorViewModel(SampleDataService dataService)
        {
            m_dataService = dataService;
            Test = "Binding test";
            SaveTaskCommand = new RelayCommand(SaveTask);
        }

        private void SaveTask()
        {
            var data = "task data to save";

            Saving = true; // Show saving dialog (and disable all controls)

            m_dataService.SaveTask(data, exception =>
            {
                Saving = false;

                if (exception != null)
                {
                    // Handle error
                }
                else
                {
                    // Task is successfully saved
                    GoBack(); // Leave editor
                }
            });
        }

        public string Test
        {
            get { return m_test; }
            set
            {
                m_test = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand SaveTaskCommand { get; private set; }
    }
}
