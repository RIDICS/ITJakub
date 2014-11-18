using GalaSoft.MvvmLight;

namespace ITJakub.MobileApps.Client.SampleApp
{
    public class SampleEditorViewModel : ViewModelBase
    {
        private string m_test;

        public SampleEditorViewModel(SampleDataService dataService)
        {
            Test = "Binding test";
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
    }
}
