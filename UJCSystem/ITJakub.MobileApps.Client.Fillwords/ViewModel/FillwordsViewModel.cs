using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Fillwords.DataService;
using ITJakub.MobileApps.Client.Shared;
using ITJakub.MobileApps.Client.Shared.Data;

namespace ITJakub.MobileApps.Client.Fillwords.ViewModel
{
    public class FillwordsViewModel : ApplicationBaseViewModel
    {
        private readonly FillwordsDataService m_dataService;
        private string m_taskDocumentRtf;
        private List<OptionsViewModel> m_taskOptionsList;
        private ObservableCollection<UserResultViewModel> m_resultList;
        private bool m_isOver;

        public FillwordsViewModel(FillwordsDataService dataService)
        {
            m_dataService = dataService;

            AnswerChangedCommand = new RelayCommand<OptionsViewModel>(AnswerChanged);
            SubmitCommand = new RelayCommand(Submit);
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

        public ObservableCollection<UserResultViewModel> ResultList
        {
            get { return m_resultList; }
            set
            {
                m_resultList = value;
                RaisePropertyChanged();
            }
        }

        public bool IsOver
        {
            get { return m_isOver; }
            set
            {
                m_isOver = value;
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

            //TODO only for test:
            IsOver = true;
            ResultList = new ObservableCollection<UserResultViewModel>
            {
                new UserResultViewModel
                {
                    CorrectAnswers = 5,
                    TotalAnswers = 16,
                    UserInfo = new AuthorInfo
                    {
                        FirstName = "Josef",
                        LastName = "Josefovič"
                    }
                },
                new UserResultViewModel
                {
                    CorrectAnswers = 9,
                    TotalAnswers = 16,
                    UserInfo = new AuthorInfo
                    {
                        FirstName = "Josef",
                        LastName = "Josefovič"
                    }
                }
            };

            //TODO save evaluation
        }
    }
}
