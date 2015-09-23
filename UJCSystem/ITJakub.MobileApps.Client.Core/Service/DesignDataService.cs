using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using ITJakub.MobileApps.Client.Core.Manager.Application;
using ITJakub.MobileApps.Client.Core.Manager.Groups;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.Core.ViewModel.Authentication;
using ITJakub.MobileApps.Client.Core.ViewModel.News;
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

        public void PromoteUserToTeacherRole(long userId, string promotionCode, Action<bool, Exception> callback)
        {
            callback(true, null);
        }

        public void GetLoggedUserInfo(bool getUserAvatar, Action<LoggedUserViewModel> callback)
        {
            callback(new LoggedUserViewModel {FirstName = "Test", LastName = "Testovaci", UserRole = UserRoleContract.Teacher});
        }

        public void LogOut()
        {
        }

        public void GetAllApplications(Action<Dictionary<ApplicationType, ApplicationBase>, Exception> callback)
        {
            callback(new Dictionary<ApplicationType, ApplicationBase>(), null);
        }

        public void GetApplication(ApplicationType type, Action<ApplicationBase, Exception> callback)
        {
            callback(null, null);
        }

        public void GetApplicationByTypes(IEnumerable<ApplicationType> types, Action<Dictionary<ApplicationType, ApplicationBase>, Exception> callback)
        {
            callback(new Dictionary<ApplicationType, ApplicationBase>(), null);
        }

        public void GetGroupDetails(long groupId, Action<GroupInfoViewModel, Exception> callback)
        {
            callback(new GroupInfoViewModel
            {
                CreateTime = new DateTime(),
                GroupCode = "Code",
                GroupName = "Název",
                Members = new ObservableCollection<GroupMemberViewModel>(),
                MemberCount = 0,
                State = GroupStateContract.AcceptMembers
            }, null);
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

        public void GetOwnedGroupsForCurrentUser(Action<List<GroupInfoViewModel>, Exception> callback)
        {
            callback(new List<GroupInfoViewModel>
            {
                new GroupInfoViewModel
                {
                    CreateTime = DateTime.Now,
                    GroupCode = "ABCDE",
                    GroupId = 124,
                    GroupName = "Moje skupinka",
                    State = GroupStateContract.Created,
                    Task = new TaskViewModel {CreateTime = DateTime.Now, Name = "testzadani"}
                }
            }, null);
        }

        public Task<ObservableCollection<GroupInfoViewModel>> GetOwnedGroupsForCurrentUserAsync()
        {
            return Task.Factory.StartNew(() => new ObservableCollection<GroupInfoViewModel>
            {
                new GroupInfoViewModel
                {
                    CreateTime = DateTime.Now,
                    GroupCode = "ABCDE",
                    GroupId = 124,
                    GroupName = "Moje skupinka",
                    State = GroupStateContract.Created,
                    Task = new TaskViewModel {CreateTime = DateTime.Now, Name = "testzadani"}
                }
            });
        }
        
        public void GetGroupsForCurrentUser(Action<List<GroupInfoViewModel>, Exception> callback)
        {
            callback(new List<GroupInfoViewModel>
            {
                new GroupInfoViewModel
                {
                    CreateTime = DateTime.Now,
                    GroupCode = "ABCDE",
                    GroupId = 124,
                    GroupName = "Moje skupinka",
                    State = GroupStateContract.Created,
                    Task = new TaskViewModel {CreateTime = DateTime.Now, Name = "testzadani"}
                }
            }, null);
        }

        public Task<ObservableCollection<GroupInfoViewModel>> GetGroupForCurrentUserAsync()
        {
            return Task.Factory.StartNew(() => new ObservableCollection<GroupInfoViewModel>
            {
                new GroupInfoViewModel
                {
                    CreateTime = DateTime.Now,
                    GroupCode = "ABCDE",
                    GroupId = 124,
                    GroupName = "Moje skupinka",
                    State = GroupStateContract.Created,
                    Task = new TaskViewModel {CreateTime = DateTime.Now, Name = "testzadani"}
                }
            });
        }

        public Task<List<SyndicationItemViewModel>> GetAllNews()
        {
            return Task.Factory.StartNew(() => new List<SyndicationItemViewModel>
            {
                new SyndicationItemViewModel
                {
                    CreateDate = DateTime.Now,
                    Title = "Byla zveřejněna nová kniha",
                    Text = "Byla zveřejněna nová kniha Jungmanův slovník pro uživatele mobilních aplikací",
                    Url = "http://censeo2.felk.cvut.cz",
                    UserEmail = "t@t.t",
                    UserFirstName = "test",
                    UserLastName = "testovaci"
                },
                new SyndicationItemViewModel
                {
                    CreateDate = DateTime.Now,
                    Title = "Nové ikony pro hangmana",
                    Text = "Nové ikony vytvořeny speciálně pro mobilní aplikaci staročeská šibenice",
                    Url = "http://censeo2.felk.cvut.cz",
                    UserEmail = "t@t.t",
                    UserFirstName = "test",
                    UserLastName = "testovaci"
                }
            });
        }

        public void CreateNewGroup(string groupName, Action<CreatedGroupViewModel, Exception> callback)
        {
            callback(new CreatedGroupViewModel {EnterCode = "ABCDEF", GroupId = 10}, null);
        }

        public void DuplicateGroup(long sourceGroupId, string groupName, Action<CreatedGroupViewModel, Exception> callback)
        {
            callback(new CreatedGroupViewModel { EnterCode = "ABCDEF", GroupId = 10 }, null);
        }

        public void ConnectToGroup(string code, Action<Exception> callback)
        {
            callback(null);
        }

        public void GetTask(long taskId, Action<TaskViewModel, Exception> callback)
        {
            callback(null, new Exception());
        }

        public void GetTaskForGroup(long groupId, Action<TaskViewModel, Exception> callback)
        {
            callback(null, new Exception());
        }

        public void GetTasksByApplication(ApplicationType application, Action<ObservableCollection<TaskViewModel>, Exception> callback)
        {
            callback(new ObservableCollection<TaskViewModel>
            {
                new TaskViewModel
                {
                    Name = "First task",
                    CreateTime = DateTime.Now,
                    Author = new UserInfo
                    {
                        FirstName = "Firstname",
                        LastName = "Lastname",
                        IsMe = true
                    }
                }
            }, null);
        }

        public void GetMyTasks(Action<ObservableCollection<TaskViewModel>, Exception> callback)
        {
            var taskList = new ObservableCollection<TaskViewModel>
            {
                new TaskViewModel
                {
                    Application = ApplicationType.Hangman,
                    Name = "Nazev 1",
                    CreateTime = DateTime.Now
                },
                new TaskViewModel
                {
                    Application = ApplicationType.Fillwords,
                    Name = "Nazev 2",
                    CreateTime = DateTime.Now.AddMinutes(-29)
                },
                new TaskViewModel
                {
                    Application = ApplicationType.Fillwords,
                    Name = "Nazev 3",
                    CreateTime = DateTime.Now
                },
                new TaskViewModel
                {
                    Application = ApplicationType.Crosswords,
                    Name = "Nazev 4",
                    CreateTime = DateTime.Now
                }
            };
            callback(taskList, null);
        }

        public void AssignTaskToCurrentGroup(long taskId, Action<Exception> callback)
        {
            callback(null);
        }

        public void SetCurrentGroup(long groupId, GroupType groupType)
        {
        }

        public void UpdateGroupState(long groupId, GroupStateContract newState, Action<Exception> callback)
        {
        }

        public void RemoveGroup(long groupId, Action<Exception> callback)
        {
        }

        public void GetCurrentGroupId(Action<long, GroupType> callback)
        {
            callback(1, GroupType.Owner);
        }

        public void SetCurrentApplication(ApplicationType selectedApp)
        {
        }

        public void GetCurrentApplication(Action<ApplicationType> callback)
        {
            callback(ApplicationType.SampleApp);
        }

        public void SetRestoringLastGroupState(bool restore)
        {
        }

        public void GetAppSelectionTarget(Action<SelectApplicationTarget> callback)
        {
            callback(SelectApplicationTarget.None);
        }

        public void SetAppSelectionTarget(SelectApplicationTarget target)
        {
        }

        public void GetGroupList(Action<List<GroupInfoViewModel>, Exception> callback)
        {
            var result = new List<GroupInfoViewModel>
            {
                new GroupInfoViewModel
                {
                    GroupCode = "123546",
                    MemberCount = 5,
                    GroupName = "Group A",
                    State = GroupStateContract.Created,
                    Task = new TaskViewModel {Application = ApplicationType.Hangman},
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
                    GroupCode = "123546",
                    MemberCount = 5,
                    GroupName = "Group B",
                    State = GroupStateContract.Running,
                    Task = new TaskViewModel {Application = ApplicationType.SampleApp}
                },
                new GroupInfoViewModel
                {
                    GroupName = "Skupina C",
                    State = GroupStateContract.WaitingForStart
                },
                new GroupInfoViewModel
                {
                    GroupName = "Skupina C",
                    State = GroupStateContract.AcceptMembers
                },
                new GroupInfoViewModel
                {
                    GroupName = "Skupina C",
                    State = GroupStateContract.Closed
                },
                new GroupInfoViewModel
                {
                    GroupName = "Skupina C",
                    State = GroupStateContract.Paused
                }
            };
            callback(result, null);
        }
		public void UpdateEndpointAddress(string address)
        {
        }

        public void RenewCodeForGroup(long groupId, Action<string, Exception> callback)
        {
            callback("1234654987", null);
        }
    }
}