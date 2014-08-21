using ITJakub.MobileApps.Client.Shared;

namespace ITJakub.MobileApps.Client.SampleApp
{
    public class SampleViewModel : ApplicationBaseViewModel
    {
        private readonly SampleDataService m_dataService;
        private string m_testString;

        public SampleViewModel(SampleDataService sampleDataService)
        {
            m_dataService = sampleDataService;
            
            //tady nacist data pro aplikaci z dataservice apod.
            TestString = "Testovaci string z viewModelu aplikace";
            LoadData();
        }

        private void LoadData()
        {
            m_dataService.GetData((exception, data) =>
            {
                TestString = data;
            });
        }

        public string TestString
        {
            get { return m_testString; }
            set { m_testString = value; RaisePropertyChanged();}
        }

        public override void InitializeCommunication()
        {
            //Load data from server and start DispatcherTimer
        }

        public override void StopCommunication()
        {
            //Stop all running DispatcherTimer
        }
    }
}
