using System;
using System.Collections.ObjectModel;
using ITJakub.MobileApps.Client.Hangman.ViewModel;
using ITJakub.MobileApps.Client.Shared.Communication;

namespace ITJakub.MobileApps.Client.Hangman.DataService
{
    public interface IHangmanDataService
    {
        void StartPollingLetters(Action<ObservableCollection<GuessViewModel>, TaskInfoViewModel, Exception> callback);
        void StopPollingLetters();
        void GuessLetter(char letter, Action<Exception> callback);
        void SetTaskAndGetWord(string data, Action<TaskInfoViewModel> callback);
    }

    public class HangmanDataService : IHangmanDataService
    {
        private readonly ISynchronizeCommunication m_applicationCommunication;
        private readonly GuessManager m_guessManager;

        public HangmanDataService(ISynchronizeCommunication applicationCommunication)
        {
            m_applicationCommunication = applicationCommunication;
            m_guessManager = new GuessManager(applicationCommunication, applicationCommunication.GetPollingService());
        }

        public void StartPollingLetters(Action<ObservableCollection<GuessViewModel>, TaskInfoViewModel, Exception> callback)
        {
            m_guessManager.StartPollingLetters(callback);
        }

        public void StopPollingLetters()
        {
            m_guessManager.StopPollingLetters();
        }

        public void GuessLetter(char letter, Action<Exception> callback)
        {
            m_guessManager.GuessLetter(letter, callback);
        }

        public void SetTaskAndGetWord(string data, Action<TaskInfoViewModel> callback)
        {
            m_guessManager.SetTask(data, callback);
        }
    }
}