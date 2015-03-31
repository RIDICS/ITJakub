using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ITJakub.MobileApps.Client.Hangman.DataContract;
using ITJakub.MobileApps.Client.Hangman.ViewModel;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Data;
using ITJakub.MobileApps.Client.Shared.Enum;
using Newtonsoft.Json;

namespace ITJakub.MobileApps.Client.Hangman.DataService
{
    public class VersusGuessManager : GuessManager
    {
        private const PollingInterval ProgressPollingInterval = PollingInterval.Medium;
        private const string ProgressObjectType = "UserProgress";
        private const string LetterObjectType = "Letter";

        private readonly ISynchronizeCommunication m_synchronizeCommunication;
        private readonly IPollingService m_pollingService;
        private Action<ObservableCollection<ProgressInfoViewModel>, Exception> m_pollingCallback;

        public VersusGuessManager(ISynchronizeCommunication synchronizeCommunication)
        {
            m_synchronizeCommunication = synchronizeCommunication;
            m_pollingService = synchronizeCommunication.PollingService;
        }

        public override TaskSettingsViewModel TaskSettings
        {
            get
            {
                return new TaskSettingsViewModel
                {
                    GuessHistoryVisible = false,
                    OpponentProgressVisible = true
                };
            }
        }

        public override void StartPollingLetters(Action<ObservableCollection<GuessViewModel>, TaskInfoViewModel, Exception> callback)
        {
            GetGuessHistory(callback);
        }

        public override void StartPollingProgress(Action<ObservableCollection<ProgressInfoViewModel>, Exception> callback)
        {
            m_pollingService.RegisterForSynchronizedObjects(ProgressPollingInterval, ApplicationType.Hangman, ProgressObjectType, ProcessNewProgressUpdate);
            m_pollingCallback = callback;
        }

        public override void StopPolling()
        {
            m_pollingService.UnregisterForSynchronizedObjects(ProgressPollingInterval, ProcessNewProgressUpdate);
        }

        public override void GuessLetter(char letter, Action<TaskInfoViewModel, Exception> callback)
        {
            var wordOrder = MyTask.WordOrder;
            MyTask.Guess(letter);
            
            var taskInfo = GetCurrentTaskInfo();
            callback(taskInfo, null);

            SendProgressInfo(callback);
            SendLetterInfo(letter, wordOrder, callback);
        }

        public override async void SaveTask(string taskName, IEnumerable<AnswerViewModel> answerList, Action<Exception> callback)
        {
            var specialLettersGenerator = new SpecialLettersGenerator();
            var wordArray = answerList.Select(model => model.Answer.ToLower()).ToArray();
            var specialLetters = specialLettersGenerator.GetSpecialLettersWithRandom(wordArray).ToArray();

            var taskContract = new HangmanTaskContract
            {
                Words = wordArray,
                SpecialLetters = specialLetters
            };

            var serializedContract = JsonConvert.SerializeObject(taskContract);

            try
            {
                await m_synchronizeCommunication.CreateTaskAsync(ApplicationType.Hangman, taskName, serializedContract);
                callback(null);
            }
            catch (ClientCommunicationException exception)
            {
                callback(exception);
            }
        }

        private async void SendProgressInfo(Action<TaskInfoViewModel, Exception> callback)
        {
            var progressUpdate = new ProgressInfoContract
            {
                LetterCount = MyTask.GuessedLetterCount,
                Lives = MyTask.Lives,
                Win = MyTask.Win
            };
            var serializedProgressInfo = JsonConvert.SerializeObject(progressUpdate);

            try
            {
                await m_synchronizeCommunication.SendObjectAsync(ApplicationType.Hangman, ProgressObjectType, serializedProgressInfo);
            }
            catch (ClientCommunicationException exception)
            {
                callback(null, exception);
            }
        }

        private async void SendLetterInfo(char letter, int wordOrder, Action<TaskInfoViewModel, Exception> callback)
        {
            var guessLetterContract = new GuessLetterContract
            {
                Letter = letter,
                WordOrder = wordOrder
            };
            var serializedGuessLetter = JsonConvert.SerializeObject(guessLetterContract);

            try
            {
                await m_synchronizeCommunication.SendObjectAsync(ApplicationType.Hangman, LetterObjectType, serializedGuessLetter);
            }
            catch (ClientCommunicationException exception)
            {
                callback(null, exception);
            }
        }

        private async void GetGuessHistory(Action<ObservableCollection<GuessViewModel>, TaskInfoViewModel, Exception> callback)
        {
            try
            {
                var result = await m_synchronizeCommunication.GetObjectsAsync(ApplicationType.Hangman, new DateTime(1970, 1, 1), LetterObjectType);
                var myObjects = result.Where(details => details.Author.IsMe);

                var list = new ObservableCollection<GuessViewModel>();

                foreach (var details in myObjects)
                {
                    var guessLetterContract = JsonConvert.DeserializeObject<GuessLetterContract>(details.Data);
                    MyTask.Guess(guessLetterContract);

                    list.Add(new GuessViewModel
                    {
                        Author = details.Author,
                        Letter = Char.ToUpper(guessLetterContract.Letter),
                        WordOrder = guessLetterContract.WordOrder
                    });

                    if (MyTask.IsNewWord)
                    {
                        callback(list, GetCurrentTaskInfo(), null);
                        list = new ObservableCollection<GuessViewModel>();
                    }
                }
                callback(list, GetCurrentTaskInfo(), null);
            }
            catch (ClientCommunicationException exception)
            {
                callback(null, null, exception);
            }
        }

        private void ProcessNewProgressUpdate(IList<ObjectDetails> objectList, Exception exception)
        {
            if (exception != null)
            {
                m_pollingCallback(null, exception);
                return;
            }

            var updateList = new ObservableCollection<ProgressInfoViewModel>();
            foreach (var objectDetails in objectList)
            {
                var progressUpdate = JsonConvert.DeserializeObject<ProgressInfoContract>(objectDetails.Data);
                var viewModel = new ProgressInfoViewModel
                {
                    LetterCount = progressUpdate.LetterCount,
                    Lives = progressUpdate.Lives,
                    Win = progressUpdate.Win,
                    UserInfo = objectDetails.Author,
                    Time = objectDetails.CreateTime
                };
                updateList.Add(viewModel);
            }

            m_pollingCallback(updateList, null);
        }
    }
}