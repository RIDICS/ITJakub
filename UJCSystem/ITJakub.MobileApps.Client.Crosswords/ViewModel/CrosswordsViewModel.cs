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

            SimpleCrosswordsViewModel = new SimpleCrosswordsViewModel(dataService);
        }

        public SimpleCrosswordsViewModel SimpleCrosswordsViewModel { get; set; }

        public override void InitializeCommunication()
        {
            //TODO start polling
            SetDataLoaded();
        }

        public override void SetTask(string data)
        {
            m_dataService.SetTaskAndGetConfiguration(data, crosswordRowViewModels =>
            {
                SimpleCrosswordsViewModel.Crossword = crosswordRowViewModels;
            });
        }

        public override void StopCommunication()
        {
            //TODO stop polling
        }
    }
}