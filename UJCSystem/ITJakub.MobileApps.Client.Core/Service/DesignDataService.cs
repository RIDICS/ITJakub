using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Media.Imaging;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.Core.ViewModel.Authentication;
using ITJakub.MobileApps.Client.Shared;
using ITJakub.MobileApps.Client.Shared.Data;
using ITJakub.MobileApps.Client.Shared.Enum;
using ITJakub.MobileApps.DataContracts;
using ITJakub.MobileApps.DataContracts.Groups;

namespace ITJakub.MobileApps.Client.Core.Service
{
    public class DesignDataService : IDataService
    {

        public void Login(AuthProvidersContract loginProviderType, Action<bool, Exception> callback)
        {
            callback(true, null);
        }

        public void CreateUser(AuthProvidersContract loginProviderType, Action<bool, Exception> callback)
        {
            callback(true, null);
        }

        public void GetLoggedUserInfo(Action<LoggedUserViewModel, Exception> callback)
        {
            callback(new LoggedUserViewModel {FirstName = "Test", LastName = "Testovaci"}, null);
        }

        public void LogOut() { }

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
                    ApplicationName = "Hangman",
                    Members = new ObservableCollection<GroupMemberViewModel>
                    {
                        new GroupMemberViewModel
                        {
                            FirstName = "Name",
                            LastName = "Surname",
                            UserAvatar = new BitmapImage(new Uri("ms-appx:///Icon/facebook-128.png"))
                        }
                    }
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

        public void OpenGroupAndGetDetails(long groupId, Action<GroupInfoViewModel, Exception> callback)
        {
            throw new NotImplementedException();
        }

        public void GetLoginProviders(Action<List<LoginProviderViewModel>, Exception> callback)
        {
            callback(new List<LoginProviderViewModel>
            {
                new LoginProviderViewModel
                {
                    LoginProviderType = AuthProvidersContract.LiveId,
                    Name = "Live ID"
                },
                new LoginProviderViewModel
                {
                    LoginProviderType = AuthProvidersContract.Facebook,
                    Name = "Facebook"
                },
                new LoginProviderViewModel
                {
                    LoginProviderType = AuthProvidersContract.Google,
                    Name = "Google"
                }
            }, null);
        }

        public void CreateNewGroup(string groupName, Action<CreateGroupResponse, Exception> callback)
        {
            callback(new CreateGroupResponse {EnterCode = "ABCDEF"}, null);
        }

        public void ConnectToGroup(string code, Action<Exception> callback)
        {
            callback(null);
        }

        public void LoadGroupMemberAvatars(IList<GroupMemberViewModel> groupMember) { }

        public void UpdateGroupMembers(GroupInfoViewModel group) { }

        public void GetTasksByApplication(ApplicationType application, Action<ObservableCollection<TaskViewModel>, Exception> callback)
        {
            callback(new ObservableCollection<TaskViewModel>
            {
                new TaskViewModel
                {
                    Name = "First task",
                    CreateTime = DateTime.Now,
                    Author = new AuthorInfo
                    {
                        FirstName = "Firstname",
                        LastName = "Lastname",
                        IsMe = true
                    }
                }
            }, null);
        }

        public void AssignTaskToGroup(long groupId, long taskId, Action<Exception> callback)
        {
            callback(null);
        }
    }
}