using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ITJakub.MobileApps.Client.Hangman.DataContract;
using ITJakub.MobileApps.Client.Hangman.ViewModel;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Data;
using ITJakub.MobileApps.Client.Shared.Enum;
using Newtonsoft.Json;

namespace ITJakub.MobileApps.Client.Hangman.DataService
{
    public class CooperationGuessManager : GuessManager
    {
        private const PollingInterval GuessPollingInterval = PollingInterval.Medium;
        private const string GuessObjectType = "GuessLetter";

        private readonly ISynchronizeCommunication m_synchronizeCommunication;
        private readonly IPollingService m_pollingService;
        private Action<ObservableCollection<GuessViewModel>, TaskInfoViewModel, Exception> m_pollingCallback;

        public CooperationGuessManager(ISynchronizeCommunication synchronizeCommunication)
        {
            m_synchronizeCommunication = synchronizeCommunication;
            m_pollingService = synchronizeCommunication.GetPollingService();
        }

        public override TaskSettingsViewModel TaskSettings
        {
            get
            {
                return new TaskSettingsViewModel
                {
                    GuessHistoryVisible = true,
                    OpponentProgressVisible = false
                };
            }
        }

        public override void StartPollingLetters(Action<ObservableCollection<GuessViewModel>, TaskInfoViewModel, Exception> callback)
        {
            m_pollingCallback = callback;
            m_pollingService.RegisterForSynchronizedObjects(GuessPollingInterval, ApplicationType.Hangman, GuessObjectType, ProcessNewGuessLetters);
        }

        public override void StartPollingProgress(Action<ObservableCollection<ProgressInfoViewModel>, Exception> callback)
        { }

        public override void StopPolling()
        {
            m_pollingService.UnregisterForSynchronizedObjects(GuessPollingInterval, ProcessNewGuessLetters);
        }

        private void ProcessNewGuessLetters(IList<ObjectDetails> newObjects, Exception exception)
        {
            if (exception != null)
            {
                m_pollingCallback(null, null, exception);
                return;
            }

            var newLetters = new ObservableCollection<GuessViewModel>();
            foreach (var objectDetails in newObjects)
            {
                var guessLetterObject = JsonConvert.DeserializeObject<GuessLetterContract>(objectDetails.Data);
                var viewModel = new GuessViewModel
                {
                    Author = objectDetails.Author,
                    Letter = Char.ToUpper(guessLetterObject.Letter),
                    WordOrder = guessLetterObject.WordOrder
                };
                newLetters.Add(viewModel);
                MyTask.Guess(guessLetterObject);

                if (MyTask.IsNewWord)
                {
                    m_pollingCallback(newLetters, GetCurrentTaskInfo(), null);
                    newLetters.Clear();
                }
            }

            m_pollingCallback(newLetters, GetCurrentTaskInfo(), null);
        }

        public override async void GuessLetter(char letter, Action<TaskInfoViewModel, Exception> callback)
        {
            // TODO test if it works in multiplayer
            var wordOrder = MyTask.WordOrder;
            MyTask.Guess(letter);
            callback(GetCurrentTaskInfo(), null);

            var dataContract = new GuessLetterContract
            {
                Letter = letter,
                WordOrder = wordOrder
            };
            var serializedObject = JsonConvert.SerializeObject(dataContract);
            try
            {
                await m_synchronizeCommunication.SendObjectAsync(ApplicationType.Hangman, GuessObjectType, serializedObject);
            }
            catch (ClientCommunicationException exception)
            {
                callback(null, exception);
            }
        }

        public override void SaveTask(string taskName, IEnumerable<AnswerViewModel> answerList, Action<Exception> callback)
        {
            throw new NotImplementedException();
        }
    }
}