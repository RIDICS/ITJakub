using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Popups;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Fillwords.DataService;
using ITJakub.MobileApps.Client.Shared.ViewModel;

namespace ITJakub.MobileApps.Client.Fillwords.ViewModel
{
    public class FillwordsViewModel : ApplicationBaseViewModel
    {
        private readonly FillwordsDataService m_dataService;
        private string m_taskDocumentRtf;
        private ObservableCollection<OptionsViewModel> m_taskOptionsList;
        private ObservableCollection<UserResultViewModel> m_resultList;
        private bool m_isOver;
        private bool m_saving;
        private bool m_isSubmitFlyoutOpen;
        private bool m_isDataLoaded;
        private bool m_isSubmited;

        public FillwordsViewModel(FillwordsDataService dataService)
        {
            m_dataService = dataService;

            SubmitCommand = new RelayCommand(Submit);
            CancelCommand = new RelayCommand(() => IsSubmitFlyoutOpen = false);
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

        public ObservableCollection<OptionsViewModel> TaskOptionsList
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

        public override void InitializeCommunication()
        {
            m_dataService.GetTaskResults((taskFinished, exception) =>
            {
                if (exception != null)
                    return;

                ResultList = taskFinished.ResultList;
                IsOver = taskFinished.IsFinished || IsOver;

                SetDataLoaded();
                m_isDataLoaded = true;
                m_isSubmited = taskFinished.IsFinished;
                
                if (IsOver)
                    StartPollingResults();
            });
        }
        
        public override void SetTask(string data)
        {
            m_dataService.SetTaskAndGetData(data, viewModel =>
            {
                TaskDocumentRtf = viewModel.DocumentRtf;
                TaskOptionsList = viewModel.Options;
            });
        }

        public override async void EvaluateAndShowResults()
        {
            IsOver = true;
            
            if (m_isDataLoaded && !m_isSubmited)
            {
                await new MessageDialog("Skupina byla ukončena, tudíž dojde k automatickému vyhodnocení vyplněných odpovědí.", "Vyhodnocení odpovědí").ShowAsync();
                Submit();
            }
        }

        public override void StopCommunication()
        {
            m_dataService.StopPolling();
        }

        public override IEnumerable<ActionViewModel> ActionsWithUsers
        {
            get { return new ActionViewModel[0]; }
        }
        
        private void Submit()
        {
            IsSubmitFlyoutOpen = false;
            Saving = true;
            m_dataService.EvaluateTask((result, exception) =>
            {
                Saving = false;
                if (exception != null)
                {
                    new MessageDialog("Úlohu se nepodařilo uložit a proto ji ani nelze vyhodnotit. Zkontrolujte připojení k internetu a zkuste to znovu.", "Vyhodnocení nelze provést").ShowAsync();
                    return;
                }

                IsOver = result.IsOver;
                m_isSubmited = result.IsOver;

                StartPollingResults();
            });
        }

        private void StartPollingResults()
        {
            m_dataService.StartPollingResults((newResults, exception) =>
            {
                if (exception != null)
                    return;

                foreach (var userResultViewModel in newResults)
                {
                    ResultList.Add(userResultViewModel);
                }
            });
        }
    }
}
