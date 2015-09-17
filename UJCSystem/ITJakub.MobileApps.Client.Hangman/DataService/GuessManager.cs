using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ITJakub.MobileApps.Client.Hangman.DataContract;
using ITJakub.MobileApps.Client.Hangman.ViewModel;
using Newtonsoft.Json;

namespace ITJakub.MobileApps.Client.Hangman.DataService
{
    public abstract class GuessManager
    {
        protected HangmanTask MyTask;

        public abstract TaskSettingsViewModel TaskSettings { get; }

        public abstract void GetTaskHistoryAndStartPollingProgress(Action<TaskProgressInfoViewModel, Exception> callback, Action<ObservableCollection<ProgressInfoViewModel>, Exception> pollingCallback);
        
        public abstract void StopPolling();

        public abstract void GuessLetter(char letter, Action<TaskProgressInfoViewModel, Exception> callback);

        public void SetTask(string data, Action<TaskSettingsViewModel, TaskProgressInfoViewModel> callback)
        {
            var task = JsonConvert.DeserializeObject<HangmanTaskContract>(data);
            MyTask = new HangmanTask(task.Words);

            var taskSettings = TaskSettings;
            taskSettings.SpecialLetters = task.SpecialLetters;

            callback(taskSettings, GetCurrentTaskInfo());
        }

        protected TaskProgressInfoViewModel GetCurrentTaskInfo()
        {
            return new TaskProgressInfoViewModel
            {
                Word = MyTask.GuessedLetters,
                LastGuessedWord = MyTask.LastGuessedLetters,
                Hint = MyTask.CurrentHint,
                Lives = MyTask.LivesRemain,
                HangmanCount = MyTask.HangmanCount,
                Win = MyTask.Win,
                GuessedWordCount = MyTask.WordOrder,
                GuessedLetterCount = MyTask.GuessedLetterCount,
                IsNewWord = MyTask.IsNewWord,
                HangmanPicture = MyTask.HangmanPicture,
            };
        }

        protected TaskProgressInfoViewModel GetCurrentTaskWithKeyboardInfo()
        {
            var taskInfo = GetCurrentTaskInfo();
            taskInfo.DeactivatedKeys = MyTask.GuessAttemptLetters;
            return taskInfo;
        }
        
        public abstract void SaveTask(string taskName, string taskDescription, IEnumerable<AnswerViewModel> answerList, Action<Exception> callback);

        public void GetTaskDetail(string data, Action<ObservableCollection<TaskLevelDetailViewModel>> callback)
        {
            if (data == null)
                return;
            
            var taskContract = JsonConvert.DeserializeObject<HangmanTaskContract>(data);
            var taskLevelList = new ObservableCollection<TaskLevelDetailViewModel>();
            foreach (var taskLevel in taskContract.Words)
            {
                taskLevelList.Add(new TaskLevelDetailViewModel
                {
                    Hint = taskLevel.Hint,
                    ResultWord = new WordViewModel { Word = taskLevel.Answer.ToUpper() }
                });
            }

            callback(taskLevelList);
        }

        public void Reset()
        {
            MyTask = null;
        }
    }
}