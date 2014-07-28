using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Media.Imaging;
using ITJakub.MobileApps.Client.Core.Manager;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.Shared;

namespace ITJakub.MobileApps.Client.Core.DataService
{
    public class DesignDataService : IDataService
    {
        public void LoginAsync(LoginProvider loginProvider, Action<UserInfo, Exception> callback)
        {
            throw new NotImplementedException();
        }

        public void GetAllApplicationViewModels(Action<ObservableCollection<ApplicationBaseViewModel>, Exception> callback)
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

        public void GetAllApplications(Action<Dictionary<ApplicationType, ApplicationBase>, Exception> callback)
        {
            throw new NotImplementedException();
        }

        public void GetApplication(ApplicationType type, Action<ApplicationBase, Exception> callback)
        {
            throw new NotImplementedException();
        }

        public void GetGroupList(Action<ObservableCollection<GroupInfoViewModel>, Exception> callback)
        {
            var result = new ObservableCollection<GroupInfoViewModel>
            {
                new GroupInfoViewModel
                {
                    ApplicationType = ApplicationType.SampleApp,
                    GroupCode = "123546",
                    MemberCount = 5,
                    GroupName = "Group A",
                    Icon = new BitmapImage(new Uri("ms-appx:///Icon/facebook-128.png")),
                    ApplicationName = "Hangman"
                },
                new GroupInfoViewModel
                {
                    ApplicationType = ApplicationType.Hangman,
                    GroupCode = "123546",
                    MemberCount = 5,
                    GroupName = "Group B",
                    Icon = new BitmapImage(new Uri("ms-appx:///Icon/facebook-128.png")),
                    ApplicationName = "Hangman"
                },
            };
            callback(result, null);
        }
    }
}
