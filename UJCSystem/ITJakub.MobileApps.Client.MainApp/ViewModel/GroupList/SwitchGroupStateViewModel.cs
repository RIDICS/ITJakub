using System;
using System.Collections.Generic;
using System.Threading;
using ITJakub.MobileApps.Client.Core.Communication.Error;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.DataContracts.Groups;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel.GroupList
{
    public class SwitchGroupStateViewModel : FlyoutBaseViewModel
    {
        private readonly GroupStateContract m_groupState;
        private readonly IDataService m_dataService;
        private readonly IList<GroupInfoViewModel> m_selectedGroups;
        private readonly Action m_refreshAction;
        private readonly IErrorService m_errorService;
        private bool m_showError;

        public SwitchGroupStateViewModel(GroupStateContract groupState, IDataService dataService, IList<GroupInfoViewModel> selectedGroups, Action refreshAction, IErrorService errorService)
        {
            m_groupState = groupState;
            m_dataService = dataService;
            m_selectedGroups = selectedGroups;
            m_refreshAction = refreshAction;
            m_errorService = errorService;

            InitLabels();
        }
        
        public string InformationText { get; set; }
        public string ErrorText { get; set; }
        public string ProgressText { get; set; }
        public string SubmitText { get; set; }

        public bool ShowError
        {
            get { return m_showError; }
            set
            {
                m_showError = value;
                RaisePropertyChanged();
            }
        }

        protected override void SubmitAction()
        {
            ChangeGroupState();
        }

        private void ChangeGroupState()
        {
            InProgress = true;
            ShowError = false;
            var groupCount = m_selectedGroups.Count;
            
            foreach (var group in m_selectedGroups)
            {
                m_dataService.UpdateGroupState(group.GroupId, m_groupState, exception =>
                {
                    if (exception != null)
                    {
                        if (exception is InvalidServerOperationException)
                            InProgress = false;
                        else
                            m_errorService.ShowConnectionError();

                        ShowError = true;
                    }

                    Interlocked.Decrement(ref groupCount);
                    if (groupCount == 0)
                    {
                        lock (this)
                        {
                            if (groupCount == 0)
                            {
                                InProgress = false;
                                m_refreshAction();
                                groupCount = -1;

                                if (!ShowError)
                                    IsFlyoutOpen = false;
                            }
                        }
                    }
                });
            }
        }

        private void InitLabels()
        {
            switch (m_groupState)
            {
                case GroupStateContract.Running:
                    InformationText = "Chystáte se zmìnit stav oznaèených skupin na Spuštìno.";
                    ErrorText = "Nìkteré skupiny se nepodaøilo spustit.";
                    ProgressText = "Probíhá spouštìní";
                    SubmitText = "Spustit";
                    break;
                case GroupStateContract.Paused:
                    InformationText = "Chystáte se zmìnit stav oznaèených skupin na Pozastaveno.";
                    ErrorText = "Nìkteré skupiny se nepodaøilo pozastavit.";
                    ProgressText = "Probíhá pozastavování";
                    SubmitText = "Pozastavit";
                    break;
                case GroupStateContract.Closed:
                    InformationText = "Chystáte se zmìnit stav oznaèených skupin na Ukonèeno.";
                    ErrorText = "Nìkteré skupiny se nepodaøilo ukonèit.";
                    ProgressText = "Ukonèování";
                    SubmitText = "Ukonèit skupinu";
                    break;
            }
        }
    }
}