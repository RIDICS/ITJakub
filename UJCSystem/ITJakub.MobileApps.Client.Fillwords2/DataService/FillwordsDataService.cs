﻿using System;
using System.Collections.Generic;
using ITJakub.MobileApps.Client.Fillwords2.ViewModel;
using ITJakub.MobileApps.Client.Fillwords2.ViewModel.Data;
using ITJakub.MobileApps.Client.Shared.Communication;

namespace ITJakub.MobileApps.Client.Fillwords2.DataService
{
    public class FillwordsDataService
    {
        private readonly ISynchronizeCommunication m_applicationCommunication;
        private readonly TaskManager m_taskManager;

        public FillwordsDataService(ISynchronizeCommunication applicationCommunication)
        {
            m_applicationCommunication = applicationCommunication;
            m_taskManager = new TaskManager(applicationCommunication);
        }

        public IErrorService ErrorService
        {
            get { return m_applicationCommunication.ErrorService; }
        }

        public void CreateTask(string taskName, string taskDescription, string bookRtfContent, IList<WordOptionsViewModel> optionsList, Action<Exception> callback)
        {
            m_taskManager.CreateTask(taskName, taskDescription, bookRtfContent, optionsList, callback);
        }

        public void SetTaskAndGetData(string data, Action<TaskViewModel> callback)
        {
            m_taskManager.SetTaskAndGetData(data, callback);
        }
    }
}
