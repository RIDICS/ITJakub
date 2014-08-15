﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Media.Imaging;
using ITJakub.MobileApps.Client.Core.Manager.Authentication;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.Core.ViewModel.Authentication;
using ITJakub.MobileApps.Client.Shared;
using ITJakub.MobileApps.Client.Shared.Enum;
using ITJakub.MobileApps.DataContracts;
using ITJakub.MobileApps.DataContracts.Groups;

namespace ITJakub.MobileApps.Client.Core.DataService
{
    public class DesignDataService : IDataService
    {

        public void Login(AuthProvidersContract loginProviderType, Action<bool, Exception> callback)
        {
            throw new NotImplementedException();
        }

        public void CreateUser(AuthProvidersContract loginProviderType, Action<bool, Exception> callback)
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
            throw new NotImplementedException();
        }

        public void ConnectToGroup(string code, Action<Exception> callback)
        {
            throw new NotImplementedException();
        }

        public void LoadGroupMemberAvatars(IList<GroupMemberViewModel> groupMember)
        {
            throw new NotImplementedException();
        }

        public void UpdateGroupMembers(GroupInfoViewModel group)
        {
            throw new NotImplementedException();
        }

        public void GetLogedUserInfo(Action<UserLoginSkeleton, Exception> callback)
        {
            throw new NotImplementedException();
        }
    }
}