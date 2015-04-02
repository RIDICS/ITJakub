using System;
using System.Collections.ObjectModel;
using ITJakub.MobileApps.Client.Chat.ViewModel;
using ITJakub.MobileApps.Client.Shared.Communication;

namespace ITJakub.MobileApps.Client.Chat.DataService
{
    public interface IChatDataService
    {
        void GetAllChatMessages(Action<ObservableCollection<MessageViewModel>, Exception> callback);
        void GetChatMessages(DateTime since, Action<ObservableCollection<MessageViewModel>, Exception> callback);
        void SendMessage(string message, Action<Exception> callback);
        void StartChatMessagesPolling(Action<ObservableCollection<MessageViewModel>, Exception> callback);
        void StopPolling();
        void UpdateMessagePollingInterval(PollingInterval pollingInterval);
    }

    public class ChatDataService : IChatDataService
    {
        private readonly ChatManager m_chatManager;

        public ChatDataService(ISynchronizeCommunication applicationCommunication)
        {
            m_chatManager = new ChatManager(applicationCommunication);
        }

        public void GetAllChatMessages(Action<ObservableCollection<MessageViewModel>, Exception> callback)
        {
            m_chatManager.GetAllChatMessages(callback);
        }

        public void GetChatMessages(DateTime since, Action<ObservableCollection<MessageViewModel>, Exception> callback)
        {
            m_chatManager.GetMessages(since, callback);
        }

        public void SendMessage(string message, Action<Exception> callback)
        {
            m_chatManager.SendMessage(message, callback);
        }

        public void StartChatMessagesPolling(Action<ObservableCollection<MessageViewModel>, Exception> callback)
        {
            m_chatManager.StartChatMessagesPolling(callback);
        }

        public void StopPolling()
        {
            m_chatManager.StopPolling();
        }

        public void UpdateMessagePollingInterval(PollingInterval pollingInterval)
        {
            m_chatManager.UpdatePollingInterval(pollingInterval);
        }
    }
}