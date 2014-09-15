using ITJakub.MobileApps.Client.Crosswords.DataService;
using ITJakub.MobileApps.Client.Shared;

namespace ITJakub.MobileApps.Client.Crosswords.ViewModel
{
    public class CrosswordsViewModel : ApplicationBaseViewModel
    {
        private readonly ICrosswordsDataService m_dataService;

        public CrosswordsViewModel(ICrosswordsDataService dataService)
        {
            m_dataService = dataService;

            SimpleCrosswordsViewModel = new SimpleCrosswordsViewModel();
        }

        public SimpleCrosswordsViewModel SimpleCrosswordsViewModel { get; set; }

        public override void InitializeCommunication()
        {
            //TODO start polling
        }

        public override void SetTask(string data)
        {
            SetDataLoaded();
        }

        public override void StopCommunication()
        {
            //TODO stop polling
        }
    }
}