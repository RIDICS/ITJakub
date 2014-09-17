using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ITJakub.MobileApps.Client.Crosswords.ViewModel;
using ITJakub.MobileApps.Client.Shared.Communication;

namespace ITJakub.MobileApps.Client.Crosswords.DataService
{
    public interface ICrosswordsDataService
    {
        void SetTaskAndGetConfiguration(string data, Action<ObservableCollection<CrosswordRowViewModel>> callback);
        void FillWord(int rowIndex, string word, Action<bool, Exception> callback);
        void StartPollingProgress(Action<List<ProgressUpdateViewModel>, Exception> callback);
        void StopPolling();
    }

    public class CrosswordsDataService : ICrosswordsDataService
    {
        private readonly CrosswordManager m_crosswordManager;

        public CrosswordsDataService(ISynchronizeCommunication applicationCommunication)
        {
            m_crosswordManager = new CrosswordManager(applicationCommunication);
        }

        public void SetTaskAndGetConfiguration(string data, Action<ObservableCollection<CrosswordRowViewModel>> callback)
        {
            m_crosswordManager.SetTaskAndGetConfiguration(data, callback);
        }

        public void FillWord(int rowIndex, string word, Action<bool, Exception> callback)
        {
            m_crosswordManager.FillWord(rowIndex, word, callback);
        }

        public void StartPollingProgress(Action<List<ProgressUpdateViewModel>, Exception> callback)
        {
            m_crosswordManager.StartPollingProgress(callback);
        }

        public void StopPolling()
        {
            m_crosswordManager.StopPolling();
        }
    }
}