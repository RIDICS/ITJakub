using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ITJakub.MobileApps.Client.Fillwords2.ViewModel;
using ITJakub.MobileApps.Client.Fillwords2.ViewModel.Data;
using ITJakub.MobileApps.Client.Shared.Communication;

namespace ITJakub.MobileApps.Client.Fillwords2.DataService
{
    public class FillwordsDataService
    {
        private readonly ISynchronizeCommunication m_applicationCommunication;
        private readonly TaskManager m_taskManager;
        private readonly ProgressManager m_progressManager;

        public FillwordsDataService(ISynchronizeCommunication applicationCommunication)
        {
            m_applicationCommunication = applicationCommunication;
            m_taskManager = new TaskManager(applicationCommunication);
            m_progressManager = new ProgressManager(applicationCommunication);
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

        public void EvaluateTask(Action<EvaluationResultViewModel, Exception> callback)
        {
            m_taskManager.EvaluateTask(callback);
        }

        public void GetTaskResults(Action<TaskFinishedViewModel, Exception> callback)
        {
            m_taskManager.GetTaskResults(callback);
        }

        public void StartPollingResults(Action<ObservableCollection<UserResultViewModel>, Exception> callback)
        {
            //m_taskManager.StartPollingResults(callback);
        }

        public void StopPolling()
        {
            //m_taskManager.StopPolling();
        }

        public void SendAnswer(int wordPosition, IList<LetterOptionViewModel> selectedAnswers, Action<Exception> callback)
        {
            m_progressManager.SendAnswer(wordPosition, selectedAnswers, callback);
        }

        public void GetAnswers(Action<IList<AnswerViewModel>, Exception> callback)
        {
            m_progressManager.GetAnswers(callback);
        }
    }
}
