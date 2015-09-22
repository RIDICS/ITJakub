using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ITJakub.MobileApps.Client.Crosswords.DataContract;
using ITJakub.MobileApps.Client.Crosswords.ViewModel;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Data;
using ITJakub.MobileApps.Client.Shared.Enum;
using Newtonsoft.Json;

namespace ITJakub.MobileApps.Client.Crosswords.DataService
{
    public class CrosswordManager
    {
        private const string ProgressMessage = "ProgressMessage";
        private const PollingInterval ProgressPollingInterval = PollingInterval.Medium;

        private readonly ISynchronizeCommunication m_applicationCommunication;
        private readonly IPollingService m_pollingService;
        private CrosswordTask m_task;
        private Action<List<ProgressUpdateViewModel>, Exception> m_pollingCallback;
        private DateTime m_lastDateTime;
        private CrosswordTaskContract m_taskContract;

        public CrosswordManager(ISynchronizeCommunication applicationCommunication)
        {
            m_applicationCommunication = applicationCommunication;
            m_pollingService = m_applicationCommunication.PollingService;
        }

        public void SetTask(string data)
        {
            m_taskContract = JsonConvert.DeserializeObject<CrosswordTaskContract>(data);
            m_task = new CrosswordTask(m_taskContract);
        }

        public void GetConfiguration(Action<ObservableCollection<CrosswordRowViewModel>> callback, bool fillAnswers)
        {
            var crosswordRows = new ObservableCollection<CrosswordRowViewModel>();

            var rowIndex = 0;
            foreach (var row in m_taskContract.RowList)
            {
                CrosswordRowViewModel crosswordRow;
                if (row.Answer != null && row.StartPosition != null)
                {
                    crosswordRow = new CrosswordRowViewModel(row.Label, row.Answer.Length, row.StartPosition.Value, m_taskContract.AnswerPosition, rowIndex++);

                    if (fillAnswers)
                        crosswordRow.UpdateWord(row.Answer);
                }
                else
                {
                    crosswordRow = new CrosswordRowViewModel();
                }
                crosswordRows.Add(crosswordRow);
            }
            
            callback(crosswordRows);
        }

        public async void FillWord(int rowIndex, string word, Action<GameInfoViewModel, Exception> callback)
        {
            m_task.FillWord(rowIndex, word);
            var isFilledCorrectly = m_task.IsRowFilledCorrectly(rowIndex);
            var gameInfo = new GameInfoViewModel
            {
                Win = m_task.Win,
                WordFilledCorrectly = isFilledCorrectly
            };

            callback(gameInfo, null);

            var messageProgress = new ProgressInfoContract
            {
                FilledWord = word,
                RowIndex = rowIndex,
                IsCorrect = isFilledCorrectly
            };

            try
            {
                await m_applicationCommunication.SendObjectAsync(ApplicationType.Crosswords, ProgressMessage, JsonConvert.SerializeObject(messageProgress));
            }
            catch (ClientCommunicationException exception)
            {
                callback(null, exception);
            }
        }
        
        public async void GetGuessHistory(Action<List<ProgressUpdateViewModel>, Exception> callback)
        {
            try
            {
                ResetLastRequestTime();
                var syncObjList = await m_applicationCommunication.GetObjectsAsync(ApplicationType.Crosswords, m_lastDateTime, ProgressMessage);
                var progressUpdateList = ProcessProgressUpdate(syncObjList);

                if (progressUpdateList.Count > 0)
                {
                    m_lastDateTime = progressUpdateList.Last().Time;
                }
                callback(progressUpdateList, null);
            }
            catch (ClientCommunicationException exception)
            {
                callback(null, exception);
            }
        }

        public void StartPollingProgress(Action<List<ProgressUpdateViewModel>, Exception> callback)
        {
            m_pollingCallback = callback;
            m_pollingService.RegisterForSynchronizedObjects(ProgressPollingInterval, ApplicationType.Crosswords, m_lastDateTime, ProgressMessage, ProgressUpdate);
        }

        private void ProgressUpdate(IList<ObjectDetails> objectList, Exception exception)
        {
            if (exception != null)
            {
                m_pollingCallback(null, exception);
                return;
            }

            var progressUpdateList = ProcessProgressUpdate(objectList);
            m_pollingCallback(progressUpdateList, null);
        }

        private List<ProgressUpdateViewModel> ProcessProgressUpdate(IList<ObjectDetails> objectList)
        {
            var progressUpdateList = new List<ProgressUpdateViewModel>();
            foreach (var objectDetails in objectList)
            {
                var updateContract = JsonConvert.DeserializeObject<ProgressInfoContract>(objectDetails.Data);
                var update = new ProgressUpdateViewModel
                {
                    FilledWord = updateContract.FilledWord,
                    RowIndex = updateContract.RowIndex,
                    IsCorrect = updateContract.IsCorrect,
                    Time = objectDetails.CreateTime,
                    UserInfo = objectDetails.Author
                };
                progressUpdateList.Add(update);

                if (objectDetails.Author.IsMe)
                    m_task.FillWord(updateContract.RowIndex, updateContract.FilledWord);
            }
            return progressUpdateList;
        }

        public void StopPolling()
        {
            m_pollingService.UnregisterForSynchronizedObjects(ProgressPollingInterval, ProgressUpdate);
        }

        public void IsWin(Action<bool> callback)
        {
            callback(m_task.Win);
        }

        public async void SaveTask(string taskName, string taskDescription, IEnumerable<EditorItemViewModel> answerList, int answerColumn, Action<Exception> callback)
        {
            try
            {
                var rowList = new List<CrosswordTaskContract.RowContract>();
                foreach (var viewModel in answerList)
                {
                    if (!viewModel.IsAnswer)
                    {
                        rowList.Add(new CrosswordTaskContract.RowContract());
                    }
                    else
                    {
                        var rowContract = new CrosswordTaskContract.RowContract
                        {
                            Answer = viewModel.Answer,
                            Label = viewModel.Label,
                            StartPosition = viewModel.Shift
                        };
                        rowList.Add(rowContract);
                    }
                }

                var taskContract = new CrosswordTaskContract
                {
                    AnswerPosition = answerColumn,
                    RowList = rowList
                };
                var serializerSettings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };

                var serializedTask = JsonConvert.SerializeObject(taskContract, Formatting.None, serializerSettings);
                await m_applicationCommunication.CreateTaskAsync(ApplicationType.Crosswords, taskName, taskDescription, serializedTask);

                callback(null);
            }
            catch (ClientCommunicationException exception)
            {
                callback(exception);
            }
        }

        public void ResetLastRequestTime()
        {
            m_lastDateTime = new DateTime(1975, 1, 1);
        }
    }
}
