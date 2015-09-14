using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ITJakub.MobileApps.Client.Crosswords.ViewModel;
using ITJakub.MobileApps.Client.Shared.Communication;

namespace ITJakub.MobileApps.Client.Crosswords.DataService
{
    public interface ICrosswordsDataService
    {
        IErrorService ErrorService { get; }
        void SetTaskAndGetConfiguration(string data, Action<ObservableCollection<CrosswordRowViewModel>> callback, bool fillAnswers = false);
        void FillWord(int rowIndex, string word, Action<GameInfoViewModel, Exception> callback);
        void GetGuessHistory(Action<List<ProgressUpdateViewModel>, Exception> callback);
        void StartPollingProgress(Action<List<ProgressUpdateViewModel>, Exception> callback);
        void StopPolling();
        void GetIsWin(Action<bool> callback);
        void SaveTask(string taskName, string taskDescription, IEnumerable<EditorItemViewModel> answerList, int answerColumn, Action<Exception> callback);
        void ResetLastRequestTime();
    }

    public class CrosswordsDataService : ICrosswordsDataService
    {
        private readonly CrosswordManager m_crosswordManager;
        private readonly IErrorService m_errorService;

        public CrosswordsDataService(ISynchronizeCommunication applicationCommunication)
        {
            m_errorService = applicationCommunication.ErrorService;
            m_crosswordManager = new CrosswordManager(applicationCommunication);
        }

        public IErrorService ErrorService
        {
            get { return m_errorService; }
        }

        public void SetTaskAndGetConfiguration(string data, Action<ObservableCollection<CrosswordRowViewModel>> callback, bool fillAnswers = false)
        {
            m_crosswordManager.SetTaskAndGetConfiguration(data, callback, fillAnswers);
        }

        public void FillWord(int rowIndex, string word, Action<GameInfoViewModel, Exception> callback)
        {
            m_crosswordManager.FillWord(rowIndex, word, callback);
        }

        public void GetGuessHistory(Action<List<ProgressUpdateViewModel>, Exception> callback)
        {
            m_crosswordManager.GetGuessHistory(callback);
        }

        public void StartPollingProgress(Action<List<ProgressUpdateViewModel>, Exception> callback)
        {
            m_crosswordManager.StartPollingProgress(callback);
        }

        public void StopPolling()
        {
            m_crosswordManager.StopPolling();
        }

        public void GetIsWin(Action<bool> callback)
        {
            m_crosswordManager.IsWin(callback);
        }

        public void SaveTask(string taskName, string taskDescription, IEnumerable<EditorItemViewModel> answerList, int answerColumn, Action<Exception> callback)
        {
            m_crosswordManager.SaveTask(taskName, taskDescription, answerList, answerColumn, callback);
        }

        public void ResetLastRequestTime()
        {
            m_crosswordManager.ResetLastRequestTime();
        }
    }
}