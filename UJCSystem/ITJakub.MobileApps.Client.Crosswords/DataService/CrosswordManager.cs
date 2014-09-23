using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public CrosswordManager(ISynchronizeCommunication applicationCommunication)
        {
            m_applicationCommunication = applicationCommunication;
            m_pollingService = m_applicationCommunication.GetPollingService();
        }

        public void SetTaskAndGetConfiguration(string data, Action<ObservableCollection<CrosswordRowViewModel>> callback)
        {
            var taskContract = JsonConvert.DeserializeObject<CrosswordTaskContract>(data);
            var crosswordRows = new ObservableCollection<CrosswordRowViewModel>();
            for (int i = 0; i < taskContract.RowList.Count; i++)
            {
                var row = taskContract.RowList[i];
                crosswordRows.Add(row.Answer != null
                    ? new CrosswordRowViewModel(row.Label, row.Answer.Length, row.StartPosition, taskContract.AnswerPosition, i)
                    : new CrosswordRowViewModel());
            }
            m_task = new CrosswordTask(taskContract);

            callback(crosswordRows);
        }

        public void FillWord(int rowIndex, string word, Action<bool, Exception> callback)
        {
            m_task.FillWord(rowIndex, word);
            var isFilledCorrectly = m_task.IsRowFilledCorrectly(rowIndex);

            callback(isFilledCorrectly, null);

            var messageProgress = new ProgressInfoContract
            {
                FilledWord = word,
                RowIndex = rowIndex,
                IsCorrect = isFilledCorrectly
            };

            try
            {
                m_applicationCommunication.SendObjectAsync(ApplicationType.Crosswords, ProgressMessage, JsonConvert.SerializeObject(messageProgress));
            }
            catch (ClientCommunicationException exception)
            {
                callback(false, exception);
            }
        }

        public void StartPollingProgress(Action<List<ProgressUpdateViewModel>, Exception> callback)
        {
            m_pollingCallback = callback;
            m_pollingService.RegisterForSynchronizedObjects(ProgressPollingInterval, ApplicationType.Crosswords, ProgressMessage, ProgressUpdate);
        }

        private void ProgressUpdate(IList<ObjectDetails> objectList, Exception exception)
        {
            if (exception != null)
            {
                m_pollingCallback(null, exception);
                return;
            }

            var progressUpdateList = new List<ProgressUpdateViewModel>();
            foreach (var objectDetails in objectList)
            {
                var updateContract = JsonConvert.DeserializeObject<ProgressInfoContract>(objectDetails.Data);
                var update = new ProgressUpdateViewModel
                {
                    FilledWord = updateContract.FilledWord,
                    RowIndex = updateContract.RowIndex,
                    IsCorrect = updateContract.IsCorrect,
                    UserInfo = objectDetails.Author
                };
                progressUpdateList.Add(update);

                if (objectDetails.Author.IsMe)
                    m_task.FillWord(updateContract.RowIndex, updateContract.FilledWord);
            }
            m_pollingCallback(progressUpdateList, null);
        }

        public void StopPolling()
        {
            m_pollingService.UnregisterForSynchronizedObjects(ProgressPollingInterval, ProgressUpdate);
        }
    }
}
