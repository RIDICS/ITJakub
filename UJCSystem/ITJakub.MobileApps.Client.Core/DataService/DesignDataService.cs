using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using ITJakub.MobileApps.Client.Core.Manager;
using ITJakub.MobileApps.Client.Core.Manager.Authentication;
using ITJakub.MobileApps.Client.Core.Manager.Groups;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.Core.ViewModel.Authentication;
using ITJakub.MobileApps.Client.Shared;
using ITJakub.MobileApps.Client.Shared.Enum;

namespace ITJakub.MobileApps.Client.Core.DataService
{
    public class DesignDataService : IDataService
    {

        public void Login(LoginProviderType loginProviderType, Action<bool, Exception> callback)
        {
            throw new NotImplementedException();
        }

        public void CreateUser(LoginProviderType loginProviderType, Action<bool, Exception> callback)
        {
            throw new NotImplementedException();
        }

        public void CreateUser(LoginProviderType loginProviderType, Action<UserLoginSkeleton, Exception> callback)
        {
            throw new NotImplementedException();
        }

        public void GetLoggedUserInfo(Action<LoggedUserViewModel, Exception> callback)
        {
            callback(new LoggedUserViewModel {FirstName = "Test", LastName = "Testovaci"}, null);
        }


        public void LogOut()
        {
            throw new NotImplementedException();
        }

        public void GetAllApplicationViewModels(Action<ObservableCollection<ApplicationBaseViewModel>, Exception> callback)
        {
            throw new NotImplementedException();
        }

        public void GetAllApplications(Action<Dictionary<ApplicationType, ApplicationBase>, Exception> callback)
        {
            throw new NotImplementedException();
        }

        public void GetApplication(ApplicationType type, Action<ApplicationBase, Exception> callback)
        {
            throw new NotImplementedException();
        }

        public void GetApplicationByTypes(IEnumerable<ApplicationType> types, Action<Dictionary<ApplicationType, ApplicationBase>, Exception> callback)
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

        public void GetGroupMembers(long groupId, Action<ObservableCollection<GroupMemberViewModel>, Exception> callback)
        {
            var group = new GroupMemberViewModel
            {
                FirstName = "FirstName",
                LastName = "LastName",
                UserAvatar = new BitmapImage(new Uri("ms-appx:///Icon/facebook-128.png"))
            };
            var groupList = new ObservableCollection<GroupMemberViewModel>();
            for (int i = 0; i < 15; i++)
            {
                groupList.Add(group);
            }
            callback(groupList, null);
        }

        public void GetGroupDetails(long groupId, Action<GroupInfoViewModel, Exception> callback)
        {
            throw new NotImplementedException();
        }

        public void GetLoginProviders(Action<List<LoginProviderViewModel>, Exception> callback)
        {
            callback(new List<LoginProviderViewModel>
            {
                new LoginProviderViewModel
                {
                    LoginProviderType = LoginProviderType.LiveId,
                    Name = "Live ID"
                },
                new LoginProviderViewModel
                {
                    LoginProviderType = LoginProviderType.Facebook,
                    Name = "Facebook"
                },
                new LoginProviderViewModel
                {
                    LoginProviderType = LoginProviderType.Google,
                    Name = "Google"
                }
            }, null);
        }

        public void CreateNewGroup(string groupName, Action<CreateGroupResult, Exception> callback)
        {
            throw new NotImplementedException();
        }

        public void ConnectToGroup(string code, Action<Exception> callback)
        {
            throw new NotImplementedException();
        }

        public void GetLogedUserInfo(Action<UserLoginSkeleton, Exception> callback)
        {
            throw new NotImplementedException();
        }
    }
}