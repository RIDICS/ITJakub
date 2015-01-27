using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ITJakub.MobileApps.Client.Hangman.DataContract;
using ITJakub.MobileApps.Client.Hangman.ViewModel;
using ITJakub.MobileApps.Client.Shared.Communication;
using Newtonsoft.Json;

namespace ITJakub.MobileApps.Client.Hangman.DataService
{
    public abstract class GuessManager
    {
        public const string CooperationMode = "Cooperation";
        public const string VersusMode = "Versus";

        protected HangmanTask MyTask;

        public static GuessManager GetInstance(string appMode, ISynchronizeCommunication synchronizeCommunication)
        {
            switch (appMode)
            {
                case CooperationMode:
                    return new CooperationGuessManager(synchronizeCommunication);
                case VersusMode:
                    return new VersusGuessManager(synchronizeCommunication);
                default:
                    return new VersusGuessManager(synchronizeCommunication);
            }
        }

        public abstract TaskSettingsViewModel TaskSettings { get; }

        public abstract void StartPollingLetters(Action<ObservableCollection<GuessViewModel>, TaskInfoViewModel, Exception> callback);

        public abstract void StartPollingProgress(Action<ObservableCollection<ProgressInfoViewModel>, Exception> callback);

        public abstract void StopPolling();

        public abstract void GuessLetter(char letter, Action<TaskInfoViewModel, Exception> callback);

        public void SetTask(string data, Action<TaskSettingsViewModel, TaskInfoViewModel> callback)
        {
            var task = JsonConvert.DeserializeObject<HangmanTaskContract>(data);
            MyTask = new HangmanTask(task.Words);

            var taskSettings = TaskSettings;
            taskSettings.SpecialLetters = task.SpecialLetters;

            callback(taskSettings, GetCurrentTaskInfo());
        }

        protected TaskInfoViewModel GetCurrentTaskInfo()
        {
            return new TaskInfoViewModel
            {
                Word = MyTask.GuessedLetters,
                Lives = MyTask.Lives,
                Win = MyTask.Win,
                GuessedWordCount = MyTask.WordOrder,
                GuessedLetterCount = MyTask.GuessedLetterCount,
                IsNewWord = MyTask.IsNewWord
            };
        }

        protected IEnumerable<char> GetSpecialLetter(IEnumerable<string> wordList)
        {
            var charSet = new HashSet<char>();
            foreach (var word in wordList)
            {
                foreach (var letter in word)
                {
                    if (letter == ' ' || (letter >= 'A' && letter <= 'Z') || (letter >= 'a' && letter <= 'z'))
                        continue;
                    charSet.Add(letter);
                }
            }
            return charSet;
        } 

        public abstract void SaveTask(string taskName, IEnumerable<AnswerViewModel> answerList, Action<Exception> callback);
    }
}