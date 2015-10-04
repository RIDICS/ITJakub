using System.Collections.Generic;
using System.Collections.ObjectModel;
using ITJakub.MobileApps.Client.Fillwords2.DataService;
using ITJakub.MobileApps.Client.Fillwords2.ViewModel.Data;
using ITJakub.MobileApps.Client.Shared.ViewModel;

namespace ITJakub.MobileApps.Client.Fillwords2.ViewModel
{
    public class FillwordsViewModel : ApplicationBaseViewModel
    {
        private readonly FillwordsDataService m_dataService;
        private string m_taskDocumentRtf;
        private ObservableCollection<SimpleWordOptionsViewModel> m_taskOptionsList;

        public FillwordsViewModel(FillwordsDataService dataService)
        {
            m_dataService = dataService;
        }

        public override void InitializeCommunication(bool isUserOwner)
        {
            SetDataLoaded();
            // TODO
        }

        public override void SetTask(string data)
        {
            m_dataService.SetTaskAndGetData(data, viewModel =>
            {
                TaskDocumentRtf = viewModel.DocumentRtf;
                TaskOptionsList = viewModel.Options;

                //foreach (var optionsViewModel in TaskOptionsList)
                //{
                //    optionsViewModel.AnswerChangedCallback = AnswerChanged;
                //}
            });
        }

        public override void EvaluateAndShowResults()
        {
            
        }

        public override void StopCommunication()
        {
            
        }

        public override IEnumerable<ActionViewModel> ActionsWithUsers
        {
            get { return new ActionViewModel[0]; }
        }

        public string TaskDocumentRtf
        {
            get { return m_taskDocumentRtf; }
            set
            {
                m_taskDocumentRtf = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<SimpleWordOptionsViewModel> TaskOptionsList
        {
            get { return m_taskOptionsList; }
            set
            {
                m_taskOptionsList = value;
                RaisePropertyChanged();
            }
        }
    }
}
