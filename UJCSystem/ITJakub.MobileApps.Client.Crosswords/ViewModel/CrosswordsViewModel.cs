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
            m_dataService.StartPollingProgress((list, exception) =>
            {
                if (exception != null)
                    return;

                SimpleCrosswordsViewModel.UpdateProgress(list);
                SetDataLoaded();
            });
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
            m_dataService.StopPolling();
        }
    }
}