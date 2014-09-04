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
    public class GuessManager
    {
        private const PollingInterval GuessPollingInterval = PollingInterval.Medium;
        private const string GuessObjectType = "GuessLetter";

        private readonly ISynchronizeCommunication m_synchronizeCommunication;
        private readonly IPollingService m_pollingService;
        private Action<ObservableCollection<GuessViewModel>, TaskInfoViewModel, Exception> m_pollingCallback;
        private HangmanTask m_task;

        public GuessManager(ISynchronizeCommunication synchronizeCommunication, IPollingService pollingService)
        {
            m_synchronizeCommunication = synchronizeCommunication;
            m_pollingService = pollingService;
        }

        public void StartPollingLetters(Action<ObservableCollection<GuessViewModel>, TaskInfoViewModel, Exception> callback)
        {
            m_pollingCallback = callback;
            m_pollingService.RegisterForSynchronizedObjects(GuessPollingInterval, ApplicationType.Hangman, GuessObjectType, ProcessNewGuessLetters);
        }

        public void StopPollingLetters()
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
                var guessLetterObject = JsonConvert.DeserializeObject<GuessLetter>(objectDetails.Data);
                var viewModel = new GuessViewModel
                {
                    Author = objectDetails.Author,
                    Letter = guessLetterObject.Letter
                };
                newLetters.Add(viewModel);
                m_task.Guess(guessLetterObject);

                if (m_task.IsNewWord)
                {
                    m_pollingCallback(newLetters, GetCurrentTaskInfo(), null);
                    newLetters.Clear();
                }
            }

            m_pollingCallback(newLetters, GetCurrentTaskInfo(), null);
        }

        public async void GuessLetter(char letter, Action<Exception> callback)
        {
            var dataContract = new GuessLetter
            {
                Letter = Char.ToUpper(letter),
                WordOrder = m_task.WordOrder
            };
            var serializedObject = JsonConvert.SerializeObject(dataContract);
            try
            {
                await
                    m_synchronizeCommunication.SendObjectAsync(ApplicationType.Hangman, GuessObjectType,
                        serializedObject);
                callback(null);
            }
            catch (ClientCommunicationException exception)
            {
                callback(exception);
            }
        }

        public void SetTask(string data, Action<TaskInfoViewModel> callback)
        {
            var task = JsonConvert.DeserializeObject<HangmanTaskContract>(data);
            m_task = new HangmanTask(task.Words);

            var taskInfo = GetCurrentTaskInfo();

            callback(taskInfo);
        }

        private TaskInfoViewModel GetCurrentTaskInfo()
        {
            return new TaskInfoViewModel
            {
                Word = m_task.GuessedLetters,
                Lives = m_task.Lives,
                Win = m_task.Win,
                WordGuessed = m_task.WordOrder,
                IsNewWord = m_task.IsNewWord
            };
        }

    }
}