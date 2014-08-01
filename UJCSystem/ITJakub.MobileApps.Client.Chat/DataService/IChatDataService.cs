using System;
using System.Collections.ObjectModel;
using ITJakub.MobileApps.Client.Chat.ViewModel;
using ITJakub.MobileApps.Client.Shared.Communication;

namespace ITJakub.MobileApps.Client.Chat.DataService
{
    public interface IChatDataService
    {
        void GetAllChatMessages(Action<ObservableCollection<MessageViewModel>, Exception> callback);
    }

    public class ChatDataService : IChatDataService
    {
        private readonly ISynchronizeCommunication m_applicationCommunication;

        public ChatDataService(ISynchronizeCommunication applicationCommunication)
        {
            m_applicationCommunication = applicationCommunication;
        }

        public void GetAllChatMessages(Action<ObservableCollection<MessageViewModel>, Exception> callback)
        {
            //read from syncservice
            callback(new ObservableCollection<MessageViewModel>(), null);
        }
    }
}