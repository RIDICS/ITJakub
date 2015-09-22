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
                    OpponentProgressVisible = true
                };
            }
        }

        public override async void GetTaskHistoryAndStartPollingProgress(Action<TaskProgressInfoViewModel, Exception> callback, Action<ObservableCollection<ProgressInfoViewModel>, Exception> pollingCallback)
        {
            m_pollingCallback = pollingCallback;
            try
            {
                var result = await m_synchronizeCommunication.GetObjectsAsync(ApplicationType.Hangman, new DateTime(1975, 1, 1), ProgressObjectType);

                ProcessGuessHistory(result, callback);
                ProcessNewProgressUpdate(result, null);

                var latestSyncObject = result.LastOrDefault();
                var latestTime = latestSyncObject != null ? latestSyncObject.CreateTime : new DateTime(1975, 1, 1);
                m_pollingService.RegisterForSynchronizedObjects(ProgressPollingInterval, ApplicationType.Hangman, latestTime, ProgressObjectType, ProcessNewProgressUpdate);
            }
            catch (ClientCommunicationException exception)
            {
                callback(null, exception);
            }
        }

        public override void StopPolling()
        {
            m_pollingService.UnregisterForSynchronizedObjects(ProgressPollingInterval, ProcessNewProgressUpdate);
        }

        public override void GuessLetter(char letter, Action<TaskProgressInfoViewModel, Exception> callback)
        {
            var wordOrder = MyTask.WordOrder;
            MyTask.Guess(letter);
            
            var taskInfo = GetCurrentTaskInfo();
            taskInfo.UseDelay = true;
            callback(taskInfo, null);

            SendProgressInfo(letter, wordOrder, callback);
        }

        public override async void SaveTask(string taskName, string taskDescription, IEnumerable<AnswerViewModel> answerList, Action<Exception> callback)
        {
            var specialLettersGenerator = new SpecialLettersGenerator();
            var wordArray = answerList.Select(x => new HangmanTaskContract.WordContract
            {
                Answer = x.Answer,
                Hint = x.Hint
            }).ToArray();
            var answerArray = wordArray.Select(x => x.Answer);
            var specialLetters = specialLettersGenerator.GetSpecialLettersWithRandom(answerArray).ToArray();

            var taskContract = new HangmanTaskContract
            {
                Words = wordArray,
                SpecialLetters = specialLetters
            };

            var serializedContract = JsonConvert.SerializeObject(taskContract);

            try
            {
                await m_synchronizeCommunication.CreateTaskAsync(ApplicationType.Hangman, taskName, taskDescription, serializedContract);
                callback(null);
            }
            catch (ClientCommunicationException exception)
            {
                callback(exception);
            }
        }

        private async void SendProgressInfo(char letter, int wordOrder, Action<TaskProgressInfoViewModel, Exception> callback)
        {
            var progressUpdate = new ProgressInfoContract
            {
                LetterCount = MyTask.GuessedLetterCount,
                WordOrder = MyTask.WordOrder, // TODO check if it has same meaning like wordOrderParameter
                HangmanCount = MyTask.HangmanCount,
                LivesRemain = MyTask.LivesRemain,
                Win = MyTask.Win,
                HangmanPicture = MyTask.HangmanPicture,
                Letter = letter
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

        private void ProcessGuessHistory(IList<ObjectDetails> syncObjectList, Action<TaskProgressInfoViewModel, Exception> callback)
        {
            if (MyTask == null)
            {
                callback(null, null);
                return;
            }

            var myObjects = syncObjectList.Where(details => details.Author.IsMe);
                
            foreach (var details in myObjects)
            {
                var guessLetterContract = JsonConvert.DeserializeObject<ProgressInfoContract>(details.Data);
                MyTask.Guess(guessLetterContract);

                if (MyTask.IsNewWord)
                {
                    callback(GetCurrentTaskInfo(), null);
                }
            }
            callback(GetCurrentTaskWithKeyboardInfo(), null);
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
                    GuessedWordCount = progressUpdate.WordOrder,
                    LetterCount = progressUpdate.LetterCount,
                    HangmanCount = progressUpdate.HangmanCount,
                    LivesRemain = progressUpdate.LivesRemain,
                    Win = progressUpdate.Win,
                    UserInfo = objectDetails.Author,
                    Time = objectDetails.CreateTime,
                    HangmanPicture = progressUpdate.HangmanPicture
                };
                updateList.Add(viewModel);
            }

            m_pollingCallback(updateList, null);
        }
    }
}