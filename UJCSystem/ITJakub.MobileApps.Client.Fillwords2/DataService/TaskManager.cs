using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ITJakub.MobileApps.Client.Fillwords2.DataContract;
using ITJakub.MobileApps.Client.Fillwords2.ViewModel;
using ITJakub.MobileApps.Client.Fillwords2.ViewModel.Data;
using ITJakub.MobileApps.Client.Fillwords2.ViewModel.Enum;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Enum;
using Newtonsoft.Json;

namespace ITJakub.MobileApps.Client.Fillwords2.DataService
{
    public class TaskManager
    {
        private const string EvaluationMessageType = "Evaluation";
        private const PollingInterval ResultsPollingInterval = PollingInterval.Slow;

        private readonly ISynchronizeCommunication m_applicationCommunication;
        private bool m_currentTaskFinished;
        private TaskViewModel m_currentTaskViewModel;

        public TaskManager(ISynchronizeCommunication applicationCommunication)
        {
            m_applicationCommunication = applicationCommunication;
        }

        public async void CreateTask(string taskName, string taskDescription, string bookRtfContent, IList<WordOptionsViewModel> wordOptionsList, Action<Exception> callback)
        {
            var taskContract = new FillwordsTaskContract
            {
                DocumentRtf = bookRtfContent,
                Options = new List<FillwordsTaskContract.WordOptionsTaskContract>()
            };

            foreach (var wordOption in wordOptionsList)
            {
                var wordOptionContract = new FillwordsTaskContract.WordOptionsTaskContract
                {
                    WordPosition = wordOption.WordPosition,
                    CorrectAnswer = wordOption.SelectedWord,
                    OptionList = new List<FillwordsTaskContract.OptionTaskContract>()
                };
                taskContract.Options.Add(wordOptionContract);

                foreach (var letterOptionViewModel in wordOption.Options)
                {
                    var optionContract = new FillwordsTaskContract.OptionTaskContract
                    {
                        StartPosition = letterOptionViewModel.StartPosition,
                        EndPosition = letterOptionViewModel.EndPosition,
                        AnswerType = letterOptionViewModel.AnswerTypeViewModel.AnswerType,
                        Options = new List<string>()
                    };
                    wordOptionContract.OptionList.Add(optionContract);
                }
            }
            
            var data = JsonConvert.SerializeObject(taskContract);

            try
            {
                await m_applicationCommunication.CreateTaskAsync(ApplicationType.Fillwords2, taskName, taskDescription, data);
                callback(null);
            }
            catch (ClientCommunicationException exception)
            {
                callback(exception);
            }
        }

        public void SetTaskAndGetData(string data, Action<TaskViewModel> callback)
        {
            if (data == null)
                return;

            var taskData = JsonConvert.DeserializeObject<FillwordsTaskContract>(data);
            if (taskData == null)
                return;

            var wordOptions = new List<SimpleWordOptionsViewModel>();
            foreach (var wordOptionsContract in taskData.Options)
            {
                var wordOption = new SimpleWordOptionsViewModel
                {
                    CorrectAnswer = wordOptionsContract.CorrectAnswer,
                    WordPosition = wordOptionsContract.WordPosition,
                    Options = new ObservableCollection<LetterOptionViewModel>()
                };
                wordOptions.Add(wordOption);

                var optionList = new List<LetterOptionViewModel>();
                foreach (var optionContract in wordOptionsContract.OptionList)
                {
                    var option = new LetterOptionViewModel
                    {
                        StartPosition = optionContract.StartPosition,
                        EndPosition = optionContract.EndPosition,
                        AnswerTypeViewModel = new AnswerTypeViewModel{AnswerType = optionContract.AnswerType},
                        Options = optionContract.Options
                    };
                    optionList.Add(option);
                }
                wordOption.Options = new ObservableCollection<LetterOptionViewModel>(optionList.OrderBy(x => x.StartPosition));
            }

            m_currentTaskFinished = false;
            m_currentTaskViewModel = new TaskViewModel
            {
                DocumentRtf = taskData.DocumentRtf,
                Options = new ObservableCollection<SimpleWordOptionsViewModel>(wordOptions.OrderBy(x => x.WordPosition))
            };
            callback(m_currentTaskViewModel);
        }

        public async void EvaluateTask(Action<EvaluationResultViewModel, Exception> callback)
        {
            int correctAnswerCount = 0;
            int optionsCount = 0;
            int index = 0;
            var taskOptionsList = m_currentTaskViewModel.Options;
            var answerList = new List<string>();
            var evaluatedAnswers = new AnswerState[taskOptionsList.Count];

            foreach (var wordOptionsViewModel in taskOptionsList)
            {
                answerList.Add(wordOptionsViewModel.SelectedAnswer);

                var correctCount = 0;
                foreach (var letterOptionViewModel in wordOptionsViewModel.Options)
                {
                    var correctLetters = wordOptionsViewModel.CorrectAnswer.Substring(
                        letterOptionViewModel.StartPosition,
                        letterOptionViewModel.EndPosition - letterOptionViewModel.StartPosition);

                    if (letterOptionViewModel.SelectedAnswer == correctLetters)
                        correctCount++;
                }

                if (correctCount == 0)
                {
                    evaluatedAnswers[index] = AnswerState.Incorrect;
                }
                else if (correctCount == wordOptionsViewModel.Options.Count)
                {
                    evaluatedAnswers[index] = AnswerState.Correct;
                }
                else
                {
                    evaluatedAnswers[index] = AnswerState.PartlyCorrect;
                }

                correctAnswerCount += correctCount;
                optionsCount += wordOptionsViewModel.Options.Count;
                index++;
            }

            try
            {
                await SaveEvaluation(answerList, correctAnswerCount, optionsCount);

                index = 0;
                foreach (var optionsViewModel in taskOptionsList)
                {
                    optionsViewModel.AnswerState = evaluatedAnswers[index++];
                }

                var evaluationResult = new EvaluationResultViewModel
                {
                    IsOver = true,
                    UserResult = new UserResultViewModel
                    {
                        CorrectAnswers = correctAnswerCount,
                        TotalAnswers = optionsCount
                    }
                };

                m_currentTaskFinished = true;
                callback(evaluationResult, null);
            }
            catch (ClientCommunicationException exception)
            {
                callback(null, exception);
            }
        }

        private async Task SaveEvaluation(IList<string> answerList, int correctAnswerCount, int optionsCount)
        {
            var evaluationObject = new FillwordsEvaluationContract
            {
                CorrectAnswers = correctAnswerCount,
                OptionsCount = optionsCount,
                AnswerList = answerList
            };
            var serializedEvaluation = JsonConvert.SerializeObject(evaluationObject);

            //await m_applicationCommunication.SendObjectAsync(ApplicationType.Fillwords2, EvaluationMessageType, serializedEvaluation);
        }
    }
}