using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ITJakub.MobileApps.Client.Fillwords.DataContract;
using ITJakub.MobileApps.Client.Fillwords.ViewModel;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Enum;
using Newtonsoft.Json;

namespace ITJakub.MobileApps.Client.Fillwords.DataService
{
    public class TaskManager
    {
        private const string EvaluationMessageType = "Evaluation";
        private readonly ISynchronizeCommunication m_applicationCommunication;

        public TaskManager(ISynchronizeCommunication applicationCommunication)
        {
            m_applicationCommunication = applicationCommunication;
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
            await m_applicationCommunication.CreateTaskAsync(ApplicationType.Fillwords, taskName, data);
            callback(null);
        }

        public void SetTaskAndGetData(string data, Action<TaskViewModel> callback)
        {
            if (data == null)
                return;

            var taskData = JsonConvert.DeserializeObject<FillwordsTaskContract>(data);
            if (taskData == null)
                return;

            var viewModel = new TaskViewModel
            {
                DocumentRtf = taskData.DocumentRtf,
                Options = new List<OptionsViewModel>(taskData.Options.Select(contract => new OptionsViewModel
                {
                    CorrectAnswer = contract.CorrectAnswer,
                    WordPosition = contract.WordPosition,
                    List = new ObservableCollection<OptionViewModel>(contract.WordList.Select(s => new OptionViewModel
                    {
                        Word = s
                    }))
                }).OrderBy(model => model.WordPosition))
            };

            callback(viewModel);
        }

        public void EvaluateTask(List<OptionsViewModel> taskOptionsList, Action<EvaluationResultViewModel, Exception> callback)
        {
            int correctAnswerCount = 0;
            var answerList = new List<string>();
            foreach (var optionsViewModel in taskOptionsList)
            {
                answerList.Add(optionsViewModel.SelectedAnswer);
                if (optionsViewModel.SelectedAnswer == optionsViewModel.CorrectAnswer)
                {
                    optionsViewModel.AnswerState = AnswerState.Correct;
                    correctAnswerCount++;
                }
                else
                {
                    optionsViewModel.AnswerState = AnswerState.Incorrect;
                }
            } //TODO not confirm evaluation if exception

            try
            {
                var evaluationObject = new FillwordsEvaluationContract
                {
                    CorrectAnswers = correctAnswerCount,
                    AnswerList = answerList
                };
                var serializedEvaluation = JsonConvert.SerializeObject(evaluationObject);

                m_applicationCommunication.SendObjectAsync(ApplicationType.Fillwords, EvaluationMessageType, serializedEvaluation);

                var evaluationResult = new EvaluationResultViewModel
                {
                    IsOver = true,
                    UserResult = new UserResultViewModel
                    {
                        CorrectAnswers = correctAnswerCount,
                        TotalAnswers = taskOptionsList.Count
                    }
                };
                callback(evaluationResult, null);
            }
            catch (ClientCommunicationException exception)
            {
                callback(null, exception);
            }
        }
    }
}