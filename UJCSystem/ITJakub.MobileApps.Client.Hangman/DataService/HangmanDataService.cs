using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ITJakub.MobileApps.Client.Hangman.ViewModel;
using ITJakub.MobileApps.Client.Shared.Communication;

namespace ITJakub.MobileApps.Client.Hangman.DataService
{
    public interface IHangmanDataService
    {
        IErrorService ErrorService { get; }
        void GetTaskHistoryAndStartPollingProgress(Action<TaskProgressInfoViewModel, Exception> callback, Action<ObservableCollection<ProgressInfoViewModel>, Exception> pollingCallback);
        void StopPolling();
        void GuessLetter(char letter, Action<TaskProgressInfoViewModel, Exception> callback);
        void SetTaskAndGetConfiguration(string data, Action<TaskSettingsViewModel, TaskProgressInfoViewModel> callback);
        void SaveTask(string taskName, string taskDescription, IEnumerable<AnswerViewModel> answerList, Action<Exception> callback);
        void GetTaskDetail(string data, Action<ObservableCollection<TaskLevelDetailViewModel>> callback);
    }

    public class HangmanDataService : IHangmanDataService
    {
        private readonly ISynchronizeCommunication m_applicationCommunication;
        private readonly GuessManager m_guessManager;

        public HangmanDataService(ISynchronizeCommunication applicationCommunication)
        {
            m_applicationCommunication = applicationCommunication;
            m_guessManager = new VersusGuessManager(applicationCommunication);
        }

        public IErrorService ErrorService
        {
            get { return m_applicationCommunication.ErrorService; }
        }

        public void GetTaskHistoryAndStartPollingProgress(Action<TaskProgressInfoViewModel, Exception> callback, Action<ObservableCollection<ProgressInfoViewModel>, Exception> pollingCallback)
        {
            m_guessManager.GetTaskHistoryAndStartPollingProgress(callback, pollingCallback);
        }
        
        public void StopPolling()
        {
            m_guessManager.StopPolling();
        }

        public void GuessLetter(char letter, Action<TaskProgressInfoViewModel, Exception> callback)
        {
            m_guessManager.GuessLetter(letter, callback);
        }

        public void SetTaskAndGetConfiguration(string data, Action<TaskSettingsViewModel, TaskProgressInfoViewModel> callback)
        {
            m_guessManager.SetTask(data, callback);
        }

        public void SaveTask(string taskName, string taskDescription, IEnumerable<AnswerViewModel> answerList, Action<Exception> callback)
        {
            m_guessManager.SaveTask(taskName, taskDescription, answerList, callback);
        }

        public void GetTaskDetail(string data, Action<ObservableCollection<TaskLevelDetailViewModel>> callback)
        {
            m_guessManager.GetTaskDetail(data, callback);
        }
    }
}