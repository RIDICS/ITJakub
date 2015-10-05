using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ITJakub.MobileApps.Client.Fillwords2.DataContract;
using ITJakub.MobileApps.Client.Fillwords2.ViewModel;
using ITJakub.MobileApps.Client.Fillwords2.ViewModel.Data;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Enum;
using Newtonsoft.Json;

namespace ITJakub.MobileApps.Client.Fillwords2.DataService
{
    public class TaskManager
    {
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
    }
}