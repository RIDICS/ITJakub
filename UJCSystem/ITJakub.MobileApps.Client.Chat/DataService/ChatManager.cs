using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ITJakub.MobileApps.Client.Chat.DataContract;
using ITJakub.MobileApps.Client.Chat.ViewModel;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Data;
using ITJakub.MobileApps.Client.Shared.Enum;
using Newtonsoft.Json;

namespace ITJakub.MobileApps.Client.Chat.DataService
{
    public class ChatManager
    {
        private readonly ISynchronizeCommunication m_applicationCommunication;
        private readonly IPollingService m_pollingService;
        private Action<ObservableCollection<MessageViewModel>, Exception> m_pollingCallback;

        private const ApplicationType AppType = ApplicationType.Chat;
        private const PollingInterval MessagePollingInterval = PollingInterval.Fast;
        private const string MessageType = "ChatMessage";

        public ChatManager(ISynchronizeCommunication applicationCommunication)
        {
            m_applicationCommunication = applicationCommunication;
            m_pollingService = applicationCommunication.GetPollingService();
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
                    await m_applicationCommunication.GetObjectsAsync(AppType, since, MessageType);
                var messages = objects.Select(objectDetails => new MessageViewModel
                {
                    Author = objectDetails.Author,
                    IsMyMessage = objectDetails.Author.IsMe,
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
                await m_applicationCommunication.SendObjectAsync(AppType, MessageType, serializedMessage);
                callback(null);
            }
            catch (ClientCommunicationException exception)
            {
                callback(exception);
            }
        }

        public void StartChatMessagesPolling(Action<ObservableCollection<MessageViewModel>, Exception> callback)
        {
            m_pollingCallback = callback;
            m_pollingService.RegisterForSynchronizedObjects(MessagePollingInterval, AppType, MessageType, ProcessNewMessages);
        }

        private void ProcessNewMessages(IList<ObjectDetails> objects, Exception exception)
        {
            if (exception != null)
            {
                m_pollingCallback(null, exception);
                return;
            }
            var messages = objects.Select(objectDetails => new MessageViewModel
            {
                Author = objectDetails.Author,
                IsMyMessage = objectDetails.Author.IsMe,
                SendTime = objectDetails.CreateTime,
                Text = JsonConvert.DeserializeObject<ChatMessage>(objectDetails.Data).Text
            });
            m_pollingCallback(new ObservableCollection<MessageViewModel>(messages), null);
        }

        public void StopPolling()
        {
            m_pollingService.UnregisterForSynchronizedObjects(MessagePollingInterval, ProcessNewMessages);
        }
    }
}