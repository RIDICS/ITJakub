﻿using System;
using System.Collections.Generic;
using System.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Core.ViewModel;

namespace ITJakub.MobileApps.Client.MainApp.ViewModel.GroupList
{
    public class DeleteGroupViewModel : ViewModelBase
    {
        private readonly IDataService m_dataService;
        private readonly List<GroupInfoViewModel> m_selectedGroups;
        private readonly Action m_refreshAction;
        private bool m_inProgress;
        private bool m_showError;
        private GroupInfoViewModel m_selectedGroup;
        private int m_selectedGroupCount;
        private bool m_isFlyoutOpen;

        public DeleteGroupViewModel(IDataService dataService, List<GroupInfoViewModel> selectedGroups, Action refreshAction)
        {
            m_dataService = dataService;
            m_selectedGroups = selectedGroups;
            m_refreshAction = refreshAction;

            DeleteGroupCommand = new RelayCommand(DeleteGroup);
        }

        public GroupInfoViewModel SelectedGroup
        {
            get { return m_selectedGroup; }
            set
            {
                m_selectedGroup = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand DeleteGroupCommand { get; private set; }

        public bool InProgress
        {
            get { return m_inProgress; }
            set
            {
                m_inProgress = value;
                RaisePropertyChanged();
            }
        }

        public bool ShowError
        {
            get { return m_showError; }
            set
            {
                m_showError = value;
                RaisePropertyChanged();
            }
        }

        public int SelectedGroupCount
        {
            get { return m_selectedGroupCount; }
            set
            {
                m_selectedGroupCount = value;
                RaisePropertyChanged();
                RaisePropertyChanged(() => IsOneItemSelected);
                RaisePropertyChanged(() => IsManyItemsSelected);
            }
        }

        public bool IsOneItemSelected
        {
            get { return SelectedGroupCount == 1; }
        }

        public bool IsManyItemsSelected
        {
            get { return SelectedGroupCount > 1; }
        }

        public bool IsFlyoutOpen
        {
            get { return m_isFlyoutOpen; }
            set
            {
                m_isFlyoutOpen = value;
                RaisePropertyChanged();
            }
        }

        private void DeleteGroup()
        {
            ShowError = false;
            InProgress = true;
            var groupCount = m_selectedGroups.Count;

            foreach (var @group in m_selectedGroups)
            {
                m_dataService.RemoveGroup(group.GroupId, exception =>
                {
                    if (exception != null)
                    {
                        ShowError = true;
                        InProgress = false;
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
    }
}
