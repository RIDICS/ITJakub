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
        void StartPollingLetters(Action<ObservableCollection<GuessViewModel>, TaskInfoViewModel, Exception> callback);
        void StartPollingProgress(Action<ObservableCollection<ProgressInfoViewModel>, Exception> callback);
        void StopPolling();
        void GuessLetter(char letter, Action<TaskInfoViewModel, Exception> callback);
        void SetTaskAndGetConfiguration(string data, string appMode, Action<TaskSettingsViewModel, TaskInfoViewModel> callback);
        void SaveTask(string taskName, IEnumerable<AnswerViewModel> answerList, Action<Exception> callback);
    }

    public class HangmanDataService : IHangmanDataService
    {
        private readonly ISynchronizeCommunication m_applicationCommunication;
        private GuessManager m_guessManager;

        public HangmanDataService(ISynchronizeCommunication applicationCommunication)
        {
            m_applicationCommunication = applicationCommunication;

            // manager with default mode
            m_guessManager = GuessManager.GetInstance(string.Empty, applicationCommunication);
        }

        public IErrorService ErrorService
        {
            get { return m_applicationCommunication.ErrorService; }
        }

        public void StartPollingLetters(Action<ObservableCollection<GuessViewModel>, TaskInfoViewModel, Exception> callback)
        {
            m_guessManager.StartPollingLetters(callback);
        }

        public void StartPollingProgress(Action<ObservableCollection<ProgressInfoViewModel>, Exception> callback)
        {
            m_guessManager.StartPollingProgress(callback);
        }

        public void StopPolling()
        {
            m_guessManager.StopPolling();
        }

        public void GuessLetter(char letter, Action<TaskInfoViewModel, Exception> callback)
        {
            m_guessManager.GuessLetter(letter, callback);
        }

        public void SetTaskAndGetConfiguration(string data, string appMode, Action<TaskSettingsViewModel, TaskInfoViewModel> callback)
        {
            m_guessManager = GuessManager.GetInstance(appMode, m_applicationCommunication);
            m_guessManager.SetTask(data, callback);
        }

        public void SaveTask(string taskName, IEnumerable<AnswerViewModel> answerList, Action<Exception> callback)
        {
            m_guessManager.SaveTask(taskName, answerList, callback);
        }
    }
}