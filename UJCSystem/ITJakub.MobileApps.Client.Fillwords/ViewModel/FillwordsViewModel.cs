using System.Collections.Generic;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Fillwords.DataService;
using ITJakub.MobileApps.Client.Shared;

namespace ITJakub.MobileApps.Client.Fillwords.ViewModel
{
    public class FillwordsViewModel : ApplicationBaseViewModel
    {
        private readonly FillwordsDataService m_dataService;
        private string m_taskName;
        private string m_taskDocumentRtf;
        private List<OptionsViewModel> m_taskOptionsList;

        public FillwordsViewModel(FillwordsDataService dataService)
        {
            m_dataService = dataService;

            AnswerChangedCommand = new RelayCommand<OptionsViewModel>(AnswerChanged);
            SubmitCommand = new RelayCommand(Submit);
        }

        public string TaskName
        {
            get { return m_taskName; }
            set
            {
                m_taskName = value;
                RaisePropertyChanged();
            }
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

        public List<OptionsViewModel> TaskOptionsList
        {
            get { return m_taskOptionsList; }
            set
            {
                m_taskOptionsList = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand<OptionsViewModel> AnswerChangedCommand { get; private set; }

        public RelayCommand SubmitCommand { get; private set; }

        public override void InitializeCommunication()
        {
            //TODO start polling or get my history?
            SetDataLoaded();
        }

        public override void SetTask(string data)
        {
            m_dataService.SetTaskAndGetData(data, viewModel =>
            {
                TaskDocumentRtf = viewModel.DocumentRtf;
                TaskOptionsList = viewModel.Options;
            });
        }

        public override void StopCommunication()
        {
            //TODO stop polling?
        }

        private void AnswerChanged(OptionsViewModel options)
        {
            //TODO save current choice to syncObject
        }
        
        private void Submit()
        {
            foreach (var optionsViewModel in TaskOptionsList)
            {
                optionsViewModel.AnswerState = optionsViewModel.SelectedAnswer == optionsViewModel.CorrectAnswer
                    ? AnswerState.Correct
                    : AnswerState.Incorrect;
            }

            //TODO save evaluation
        }
    }
}
