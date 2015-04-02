using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ITJakub.MobileApps.Client.Fillwords.DataContract;
using ITJakub.MobileApps.Client.Fillwords.ViewModel;
using ITJakub.MobileApps.Client.Fillwords.ViewModel.Enum;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Data;
using ITJakub.MobileApps.Client.Shared.Enum;
using Newtonsoft.Json;

namespace ITJakub.MobileApps.Client.Fillwords.DataService
{
    public class TaskManager
    {
        private const string EvaluationMessageType = "Evaluation";
        private const PollingInterval ResultsPollingInterval = PollingInterval.VerySlow;
        private readonly ISynchronizeCommunication m_applicationCommunication;
        private readonly IPollingService m_pollingService;
        private TaskViewModel m_currentTaskViewModel;
        private bool m_currentTaskFinished;
        private DateTime m_lastUserResultTime;
        private Action<ObservableCollection<UserResultViewModel>, Exception> m_resultCallback;

        public TaskManager(ISynchronizeCommunication applicationCommunication)
        {
            m_applicationCommunication = applicationCommunication;
            m_pollingService = applicationCommunication.PollingService;
        }
        
        public async void CreateTask(string taskName, string bookRtfContent, IList<OptionsViewModel> optionsList, Action<Exception> callback)
        {
            var taskContract = new FillwordsTaskContract
            {
                DocumentRtf = bookRtfContent,
                Options = new List<FillwordsTaskContract.WordOptionsTaskContract>(optionsList.Select(model => new FillwordsTaskContract.WordOptionsTaskContract
                {
                    WordList = model.List.Select(viewModel => viewModel.Word).ToList(),
                    WordPosition = model.WordPosition,
                    CorrectAnswer = model.CorrectAnswer
                }))
            };
            var data = JsonConvert.SerializeObject(taskContract);

            try
            {
                await m_applicationCommunication.CreateTaskAsync(ApplicationType.Fillwords, taskName, data);
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

            m_currentTaskFinished = false;
            m_currentTaskViewModel = new TaskViewModel
            {
                DocumentRtf = taskData.DocumentRtf,
                Options = new ObservableCollection<OptionsViewModel>(taskData.Options.Select(contract => new OptionsViewModel
                {
                    CorrectAnswer = contract.CorrectAnswer,
                    WordPosition = contract.WordPosition,
                    List = new ObservableCollection<OptionViewModel>(contract.WordList.Select(s => new OptionViewModel
                    {
                        Word = s
                    }))
                }).OrderBy(model => model.WordPosition))
            };

            callback(m_currentTaskViewModel);
        }

        public async void EvaluateTask(Action<EvaluationResultViewModel, Exception> callback)
        {
            int correctAnswerCount = 0;
            int index = 0;
            var taskOptionsList = m_currentTaskViewModel.Options;
            var answerList = new List<string>();
            var evaluatedAnswers = new AnswerState[taskOptionsList.Count];

            foreach (var optionsViewModel in taskOptionsList)
            {
                answerList.Add(optionsViewModel.SelectedAnswer);
                
                if (optionsViewModel.SelectedAnswer == optionsViewModel.CorrectAnswer)
                {
                    evaluatedAnswers[index++] = AnswerState.Correct;
                    correctAnswerCount++;
                }
                else
                {
                    evaluatedAnswers[index++] = AnswerState.Incorrect;
                }
            }

            try
            {
                await SaveEvaluation(answerList, correctAnswerCount);

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
                        TotalAnswers = taskOptionsList.Count
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

        private async Task SaveEvaluation(IList<string> answerList, int correctAnswerCount)
        {
            var evaluationObject = new FillwordsEvaluationContract
            {
                CorrectAnswers = correctAnswerCount,
                AnswerList = answerList
            };
            var serializedEvaluation = JsonConvert.SerializeObject(evaluationObject);

            await m_applicationCommunication.SendObjectAsync(ApplicationType.Fillwords, EvaluationMessageType, serializedEvaluation);
        }

        private ObservableCollection<UserResultViewModel> ProcessResults(IList<ObjectDetails> results)
        {
            var outputCollection = new ObservableCollection<UserResultViewModel>();
            foreach (var objectDetails in results)
            {
                var deserializedObject = JsonConvert.DeserializeObject<FillwordsEvaluationContract>(objectDetails.Data);
                var userResultViewModel = new UserResultViewModel
                {
                    CorrectAnswers = deserializedObject.CorrectAnswers,
                    TotalAnswers = deserializedObject.AnswerList.Count,
                    UserInfo = objectDetails.Author
                };
                outputCollection.Add(userResultViewModel);

                if (objectDetails.Author.IsMe)
                {
                    FillMyAnswers(deserializedObject.AnswerList);
                    m_currentTaskFinished = true;
                }

                m_lastUserResultTime = objectDetails.CreateTime;
            }

            return outputCollection;
        }

        private void FillMyAnswers(IEnumerable<string> myAnswers)
        {
            var answersEnumerator = myAnswers.GetEnumerator();
            var optionsEnumerator = m_currentTaskViewModel.Options.GetEnumerator();

            while (answersEnumerator.MoveNext() && optionsEnumerator.MoveNext())
            {
                var currentOptions = optionsEnumerator.Current;
                
                currentOptions.SelectedAnswer = answersEnumerator.Current;
                currentOptions.AnswerState = currentOptions.SelectedAnswer == currentOptions.CorrectAnswer
                    ? AnswerState.Correct
                    : AnswerState.Incorrect;
            }
        }

        public async void GetTaskResults(Action<TaskFinishedViewModel, Exception> callback)
        {
            try
            {
                m_lastUserResultTime = new DateTime(1970, 1, 1);
                var results =
                    await
                        m_applicationCommunication.GetObjectsAsync(ApplicationType.Fillwords, m_lastUserResultTime,
                            EvaluationMessageType);

                var resultViewModels = ProcessResults(results);
                callback(new TaskFinishedViewModel
                {
                    IsFinished = m_currentTaskFinished,
                    ResultList = resultViewModels
                }, null);
            }
            catch (ClientCommunicationException exception)
            {
                callback(null, exception);
            }
        }

        public void StartPollingResults(Action<ObservableCollection<UserResultViewModel>, Exception> callback)
        {
            m_resultCallback = callback;
            m_pollingService.RegisterForSynchronizedObjects(ResultsPollingInterval, ApplicationType.Fillwords, m_lastUserResultTime, EvaluationMessageType, PollingResultsCallback);
        }

        private void PollingResultsCallback(IList<ObjectDetails> objectList, Exception exception)
        {
            if (exception != null)
            {
                m_resultCallback(null, exception);
                return;
            }
            
            m_resultCallback(ProcessResults(objectList), null);
        }

        public void StopPolling()
        {
            m_pollingService.UnregisterForSynchronizedObjects(ResultsPollingInterval, PollingResultsCallback);
        }
    }
}