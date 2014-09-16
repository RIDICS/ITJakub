using System;
using System.Collections.ObjectModel;
using System.Linq;
using ITJakub.MobileApps.Client.Crosswords.DataContract;
using ITJakub.MobileApps.Client.Crosswords.ViewModel;
using ITJakub.MobileApps.Client.Shared.Communication;
using Newtonsoft.Json;

namespace ITJakub.MobileApps.Client.Crosswords.DataService
{
    public class CrosswordManager
    {
        private readonly ISynchronizeCommunication m_applicationCommunication;
        private CrosswordTask m_task;

        public CrosswordManager(ISynchronizeCommunication applicationCommunication)
        {
            m_applicationCommunication = applicationCommunication;
        }

        public void SetTaskAndGetConfiguration(string data, Action<ObservableCollection<CrosswordRowViewModel>> callback)
        {
            var taskContract = JsonConvert.DeserializeObject<CrosswordTaskContract>(data);
            var crosswordRows = new ObservableCollection<CrosswordRowViewModel>();
            for (int i = 0; i < taskContract.RowList.Count; i++)
            {
                var row = taskContract.RowList[i];
                crosswordRows.Add(new CrosswordRowViewModel(row.Label, row.Answer.Length, row.StartPosition, taskContract.AnswerPosition, i));
            }
            m_task = new CrosswordTask(taskContract);

            callback(crosswordRows);
        }

        public void FillWord(int rowIndex, string word, Action<bool, Exception> callback)
        {
            m_task.FillWord(rowIndex, word);
            callback(m_task.IsRowFilledCorrectly(rowIndex), null);

            //TODO send synchronized object to server
        }
    }
}
