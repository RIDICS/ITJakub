using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ITJakub.MobileApps.Client.Fillwords.ViewModel;
using ITJakub.MobileApps.Client.Shared.Communication;

namespace ITJakub.MobileApps.Client.Fillwords.DataService
{
    public class FillwordsDataService
    {
        private readonly TaskManager m_taskManager;

        public FillwordsDataService(ISynchronizeCommunication applicationCommunication)
        {
            m_taskManager = new TaskManager(applicationCommunication);
        }

        public void CreateTask(string taskName, string bookRtfContent, IList<OptionsViewModel> optionsList, Action<Exception> callback)
        {
            m_taskManager.CreateTask(taskName, bookRtfContent, optionsList, callback);
        }

        public void SetTaskAndGetData(string data, Action<TaskViewModel> callback)
        {
            m_taskManager.SetTaskAndGetData(data, callback);
        }

        public void EvaluateTask(ICollection<OptionsViewModel> taskOptionsList, Action<EvaluationResultViewModel, Exception> callback)
        {
            m_taskManager.EvaluateTask(taskOptionsList, callback);
        }
    }
}