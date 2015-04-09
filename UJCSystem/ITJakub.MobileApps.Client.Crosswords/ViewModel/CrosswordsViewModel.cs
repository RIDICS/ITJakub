using System.Collections.Generic;
using ITJakub.MobileApps.Client.Crosswords.DataService;
using ITJakub.MobileApps.Client.Shared.ViewModel;

namespace ITJakub.MobileApps.Client.Crosswords.ViewModel
{
    public class CrosswordsViewModel : ApplicationBaseViewModel
    {
        private readonly ICrosswordsDataService m_dataService;

        public CrosswordsViewModel(ICrosswordsDataService dataService)
        {
            m_dataService = dataService;

            SimpleCrosswordsViewModel = new SimpleCrosswordsViewModel(dataService);
            SimpleCrosswordsViewModel.GoBack = GoBack;
        }

        public SimpleCrosswordsViewModel SimpleCrosswordsViewModel { get; set; }

        public override void InitializeCommunication()
        {
            // TODO try fix slow loading
            m_dataService.StartPollingProgress((list, exception) =>
            {
                if (exception != null)
                {
                    m_dataService.ErrorService.ShowConnectionWarning();
                    return;
                }

                SimpleCrosswordsViewModel.UpdateProgress(list);
                m_dataService.GetIsWin(isWin => SimpleCrosswordsViewModel.SetWin(isWin));
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

        public override void EvaluateAndShowResults()
        {
            SimpleCrosswordsViewModel.StopAndShowResults();
        }

        public override void StopCommunication()
        {
            m_dataService.StopPolling();
        }

        public override IEnumerable<ActionViewModel> ActionsWithUsers
        {
            get { return new ActionViewModel[0]; }
        }
    }
}