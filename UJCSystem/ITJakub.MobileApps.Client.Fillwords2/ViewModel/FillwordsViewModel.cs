using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
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
        private ObservableCollection<UserResultViewModel> m_resultList;
        private bool m_isOver;
        private bool m_saving;
        private bool m_isSubmitFlyoutOpen;

        public FillwordsViewModel(FillwordsDataService dataService)
        {
            m_dataService = dataService;

            SubmitCommand = new RelayCommand(Submit);
            CancelCommand = new RelayCommand(() => IsSubmitFlyoutOpen = false);
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
        private void Submit()
        {
            IsSubmitFlyoutOpen = false;
            Saving = true;
            //m_dataService.EvaluateTask((result, exception) =>
            //{
            //    Saving = false;
            //    if (exception != null)
            //    {
            //        m_dataService.ErrorService.ShowError("Úlohu se nepodařilo uložit a proto ji ani nelze vyhodnotit. Zkontrolujte připojení k internetu a zkuste to znovu.", "Vyhodnocení nelze provést");
            //        return;
            //    }

            //    IsOver = result.IsOver;
            //    m_isSubmited = result.IsOver;

            //    StartPollingResults();
            //});
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

        public bool Saving
        {
            get { return m_saving; }
            set
            {
                m_saving = value;
                RaisePropertyChanged();
            }
        }

        public bool IsSubmitFlyoutOpen
        {
            get { return m_isSubmitFlyoutOpen; }
            set
            {
                m_isSubmitFlyoutOpen = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand SubmitCommand { get; private set; }

        public RelayCommand CancelCommand { get; private set; }

    }
}
