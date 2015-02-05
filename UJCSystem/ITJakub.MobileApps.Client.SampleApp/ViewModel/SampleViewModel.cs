using ITJakub.MobileApps.Client.SampleApp.Service;
using ITJakub.MobileApps.Client.Shared.ViewModel;

namespace ITJakub.MobileApps.Client.SampleApp.ViewModel
{
    public class SampleViewModel : ApplicationBaseViewModel
    {
        private readonly SampleDataService m_dataService;
        private string m_testString;

        public SampleViewModel(SampleDataService sampleDataService)
        {
            m_dataService = sampleDataService;
            
            TestString = "Testovaci string z viewModelu aplikace";
        }

        public string TestString
        {
            get { return m_testString; }
            set { m_testString = value; RaisePropertyChanged();}
        }

        public override void InitializeCommunication()
        {
            //Load data from server and use IPollingService for getting synchronized objects
            m_dataService.GetData((data, exception) =>
            {
                TestString = data;
            });

            m_dataService.StartObjectPolling((list, exception) =>
            {
                if (exception != null)
                    return;

                //Process new synchronized objects

                //Call method SetDataLoaded to disappear loading dialog
                SetDataLoaded();
            });
        }

        public override void SetTask(string data)
        {
            //Get task data
        }

        public override void StopCommunication()
        {
            //Stop all running timers and polling requests in IPollingService
        }
    }
}
