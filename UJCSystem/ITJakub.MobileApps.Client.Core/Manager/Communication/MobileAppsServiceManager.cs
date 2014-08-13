using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using ITJakub.MobileApps.Client.Core.Manager.Authentication;
using ITJakub.MobileApps.Client.Core.Manager.Communication.Client;
using ITJakub.MobileApps.Client.Core.Manager.Groups;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Data;
using ITJakub.MobileApps.Client.Shared.Enum;
using ITJakub.MobileApps.DataContracts;
using ITJakub.MobileApps.DataContracts.Applications;
using Task = System.Threading.Tasks.Task;

namespace ITJakub.MobileApps.Client.Core.Manager.Communication
{
    //TODO zbytecna trida - presunout do Manageru
    public class MobileAppsServiceManager
    {
        private readonly MobileAppsServiceClient m_serviceClient;
        private readonly ClientMessageInspector m_clientMessageInspector;

        public MobileAppsServiceManager()
        {
            m_serviceClient = new MobileAppsServiceClient();
            m_clientMessageInspector = new ClientMessageInspector();
            var endpointBehavior = new CustomEndpointBehavior(m_clientMessageInspector);
            m_serviceClient.Endpoint.EndpointBehaviors.Add(endpointBehavior);
        }

        public void UpdateCommunicationToken(string communicationToken)
        {
            m_clientMessageInspector.CommunicationToken = communicationToken;
        }

        public async Task<LoginResult> LoginUserAsync(AuthProvidersContract loginProviderType, string email, string accessToken)
        {
            try
            {
                var response = m_serviceClient.LoginUser(loginProviderType, accessToken, email);
                var loginResult = new LoginResult
                {
                    CommunicationToken = response.CommunicationToken,
                    EstimatedExpirationTime = response.EstimatedExpirationTime,
                    UserId = response.UserId,
                    UserAvatarUrl = response.ProfilePictureUrl,
                    UserRole = response.UserRole
                };
                return loginResult;
            }
            catch (FaultException)
            {
                throw new UserNotRegisteredException();
            }
            catch (CommunicationException)
            {
                throw new ClientCommunicationException();
            }
            catch (TimeoutException)
            {
                throw new ClientCommunicationException();
            }
            catch (ObjectDisposedException)
            {
                throw new ClientCommunicationException();
            }
        }

        public async Task CreateUserAsync(AuthProvidersContract loginProviderType, UserLoginSkeleton userLoginSkeleton)
        {
            try
            {
                m_serviceClient.CreateUser(loginProviderType,userLoginSkeleton.AccessToken, new UserDetailContract
                {
                    Email = userLoginSkeleton.Email,
                    FirstName = userLoginSkeleton.FirstName,
                    LastName = userLoginSkeleton.LastName
                });
            }
                //TODO move all exceptions to Client
            catch (FaultException)
            {
                throw new ClientCommunicationException();
            }
            catch (CommunicationException)
            {
                throw new ClientCommunicationException();
            }
            catch (TimeoutException)
            {
                throw new ClientCommunicationException();
            }
            catch (ObjectDisposedException)
            {
                throw new ClientCommunicationException();
            }
        }

        public async Task<ObservableCollection<GroupInfoViewModel>> GetGroupListAsync(long userId)
        {
            try
            {
                var response = m_serviceClient.GetGroupsByUser(userId);
                var list = new ObservableCollection<GroupInfoViewModel>();
                
                foreach (var groupDetails in response.MemberOfGroup)
                {
                    list.Add(new GroupInfoViewModel
                    {
                        GroupName = groupDetails.Name,
                        MemberCount = groupDetails.Members.Count,
                        GroupId = groupDetails.Id,
                        GroupType = GroupType.Member
                    });
                }

                foreach (var groupDetails in response.OwnedGroups)
                {
                    list.Add(new GroupInfoViewModel
                    {
                        GroupName = groupDetails.Name,
                        MemberCount = groupDetails.Members.Count,
                        GroupId = groupDetails.Id,
                        GroupType = GroupType.Owner,
                        GroupCode = groupDetails.EnterCode,
                        CreateTime = groupDetails.CreateTime,
                        //TODO Hack for debug
                        ApplicationType = ApplicationType.SampleApp
                    });
                }
                return list;
            }
            catch (FaultException)
            {
                throw new ClientCommunicationException();
            }
            catch (CommunicationException)
            {
                throw new ClientCommunicationException();
            }
            catch (TimeoutException)
            {
                throw new ClientCommunicationException();
            }
            catch (ObjectDisposedException)
            {
                throw new ClientCommunicationException();
            }
        }

        public async Task<CreateGroupResult> CreateGroupAsync(long userId, string groupName)
        {
            try
            {
                var response = m_serviceClient.CreateGroup(userId, groupName);
                var result = new CreateGroupResult
                {
                    EnterCode = response.EnterCode
                };
                return result;
            }
            catch (FaultException)
            {
                throw new ClientCommunicationException();
            }
            catch (CommunicationException)
            {
                throw new ClientCommunicationException();
            }
            catch (TimeoutException)
            {
                throw new ClientCommunicationException();
            }
            catch (ObjectDisposedException)
            {
                throw new ClientCommunicationException();
            }
        }

        public async Task AddUserToGroupAsync(string code, long userId)
        {
            try
            {
                m_serviceClient.AddUserToGroup(code, userId);
            }
            catch (FaultException)
            {
                throw new ClientCommunicationException();
            }
            catch (CommunicationException)
            {
                throw new ClientCommunicationException();
            }
            catch (TimeoutException)
            {
                throw new ClientCommunicationException();
            }
            catch (ObjectDisposedException)
            {
                throw new ClientCommunicationException();
            }
        }

        public async Task<GroupInfoViewModel> GetGroupDetailsAsync(long groupId)
        {
            try
            {
                //var result = await m_serviceClient.GetGroupDetailsAsync(groupId.ToString());
                //var group = new GroupInfoViewModel
                //{
                //    GroupId = result.Id,
                //    GroupName = result.Group.Name,
                //    MemberCount = result.Members.Count,
                //    Members =
                //        new ObservableCollection<GroupMemberViewModel>(
                //            result.Members.Select(details => new GroupMemberViewModel
                //            {
                //                FirstName = details.User.FirstName,
                //                LastName = details.User.LastName
                //            }))
                //};
                //return group;
                return new GroupInfoViewModel();
            }
            catch (FaultException)
            {
                throw new ClientCommunicationException();
            }
            catch (CommunicationException)
            {
                throw new ClientCommunicationException();
            }
            catch (TimeoutException)
            {
                throw new ClientCommunicationException();
            }
            catch (ObjectDisposedException)
            {
                throw new ClientCommunicationException();
            }
        }

        public async Task SendSynchronizedObjectAsync(ApplicationType applicationType, long groupId, long userId, string objectType, string objectValue)
        {
            try
            {
                var applicationId = applicationType.ToString();
                
                var synchronizedObject = new SynchronizedObjectContract
                {
                    ObjectType = objectType,
                    Data = objectValue
                };
                m_serviceClient.CreateSynchronizedObject(1, groupId, userId, synchronizedObject);
            }
            catch (FaultException)
            {
                throw new ClientCommunicationException();
            }
            catch (CommunicationException)
            {
                throw new ClientCommunicationException();
            }
            catch (TimeoutException)
            {
                throw new ClientCommunicationException();
            }
            catch (ObjectDisposedException)
            {
                throw new ClientCommunicationException();
            }
        }

        public async Task<IEnumerable<ObjectDetails>> GetSynchronizedObjectsAsync(ApplicationType applicationType, long groupId, long userId, string objectType, DateTime since)
        {
            try
            {
                var objectList = m_serviceClient.GetSynchronizedObjects(groupId, 1, objectType, since);
                //var applicationId = ApplicationTypeConverter.ConvertToString(applicationType);
                //var dateTimeString = since.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
                //var objectList = await m_serviceClient.GetSynchronizedObjectsAsync(groupId.ToString(), applicationId, objectType, dateTimeString);
                var outputList = objectList.Select(objectDetails => new ObjectDetails
                {
                    Author = new AuthorInfo
                    {

                        Email = objectDetails.Author.Email,
                        FirstName = objectDetails.Author.FirstName,
                        LastName = objectDetails.Author.LastName,
                        Id = objectDetails.Author.Id,
                        IsMe = (userId == objectDetails.Author.Id)
                    },
                    CreateTime = objectDetails.CreateTime,
                    Data = objectDetails.Data
                });
                return outputList;
            }
            catch (FaultException)
            {
                throw new ClientCommunicationException();
            }
            catch (CommunicationException)
            {
                throw new ClientCommunicationException();
            }
            catch (TimeoutException)
            {
                throw new ClientCommunicationException();
            }
            catch (ObjectDisposedException)
            {
                throw new ClientCommunicationException();
            }
        }
    }
}
