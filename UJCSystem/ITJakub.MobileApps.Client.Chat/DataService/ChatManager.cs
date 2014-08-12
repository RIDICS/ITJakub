using System;
using System.Collections.ObjectModel;
using System.Linq;
using ITJakub.MobileApps.Client.Chat.DataContract;
using ITJakub.MobileApps.Client.Chat.ViewModel;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Enum;
using Newtonsoft.Json;

namespace ITJakub.MobileApps.Client.Chat.DataService
{
    public class ChatManager
    {
        private readonly ISynchronizeCommunication m_applicationCommunication;
        private ApplicationType m_applicationType;
        private string m_messageType;

        public ChatManager(ISynchronizeCommunication applicationCommunication)
        {
            m_applicationCommunication = applicationCommunication;
            m_applicationType = ApplicationType.Chat;
            m_messageType = "ChatMessage";
        }

        public void GetAllChatMessages(Action<ObservableCollection<MessageViewModel>, Exception> callback)
        {
            GetMessages(new DateTime(1970,1,1), callback);
        }

        public async void GetMessages(DateTime since, Action<ObservableCollection<MessageViewModel>, Exception> callback)
        {
            try
            {
                var objects =
                    await m_applicationCommunication.GetObjectsAsync(m_applicationType, since, m_messageType);
                var messages = objects.Select(objectDetails => new MessageViewModel
                {
                    Author = objectDetails.Author,
                    IsMyMessage = false, //TODO load correct
                    SendTime = objectDetails.CreateTime,
                    Text = JsonConvert.DeserializeObject<ChatMessage>(objectDetails.Data).Text
                });
                callback(new ObservableCollection<MessageViewModel>(messages), null);
            }
            catch (ClientCommunicationException exception)
            {
                callback(null, exception);
            }
        }

        public async void SendMessage(string message, Action<Exception> callback)
        {
            try
            {
                var chatMessage = new ChatMessage
                {
                    Text = message
                };
                var serializedMessage = JsonConvert.SerializeObject(chatMessage);
                await m_applicationCommunication.SendObjectAsync(m_applicationType, m_messageType, serializedMessage);
                callback(null);
            }
            catch (ClientCommunicationException exception)
            {
                callback(exception);
            }
        }
    }
}