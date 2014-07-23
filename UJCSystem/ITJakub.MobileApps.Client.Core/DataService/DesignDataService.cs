using System;
using System.Collections.ObjectModel;
using ITJakub.MobileApps.Client.Core.Manager;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.MainApp;
using ITJakub.MobileApps.Client.Shared;

namespace ITJakub.MobileApps.Client.Core.DataService
{
    public class DesignDataService : IDataService
    {
        public void LoginAsync(LoginProvider loginProvider, Action<UserInfo, Exception> callback)
        {
            throw new NotImplementedException();
        }

        public void GetAllApplicationViewModels(Action<ObservableCollection<ApplicationBaseViewModel>, object> callback)
        {
            throw new NotImplementedException();
        }

        public void GetAllChatMessages(Action<ObservableCollection<MessageViewModel>, Exception> callback)
        {
            var messages = new ObservableCollection<MessageViewModel>
            {
                new MessageViewModel
                {
                    Content = "Lorem ipsum dolor sit amet",
                    DateTime = DateTime.Now,
                    Name = "User A",
                    IsMyMessage = false
                },
                new MessageViewModel
                {
                    Content = "consectetur adipiscing elit",
                    DateTime = DateTime.Now,
                    Name = "User B",
                    IsMyMessage = true
                },
                new MessageViewModel
                {
                    Content = "Vestibulum commodo interdum nunc",
                    DateTime = DateTime.Now,
                    Name = "User C",
                    IsMyMessage = false
                },
                new MessageViewModel
                {
                    Content = "a lobortis odio semper non.",
                    DateTime = DateTime.Now,
                    Name = "User D",
                    IsMyMessage = false
                },
            };
            callback(messages, null);
        }
    }
}
