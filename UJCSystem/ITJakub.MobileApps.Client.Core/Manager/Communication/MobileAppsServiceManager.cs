using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using ITJakub.MobileApps.Client.Core.Manager.Authentication;
using ITJakub.MobileApps.Client.Core.Manager.Converter;
using ITJakub.MobileApps.Client.Core.Manager.Groups;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Data;
using ITJakub.MobileApps.Client.Shared.Enum;
using Task = System.Threading.Tasks.Task;

namespace ITJakub.MobileApps.Client.Core.Manager.Communication
{
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

        public async Task<LoginResult> LoginUserAsync(LoginProviderType loginProviderType, string email, string accessToken)
        {
            try
            {
                var authenticationProvider = LoginProviderConverter.LoginToAuthenticationProvider(loginProviderType);
                var response = await m_serviceClient.LoginUserAsync(new UserLogin
                {
                    AuthenticationProvider = authenticationProvider,
                    AuthenticationToken = accessToken,
                    Email = email
                });
                var loginResult = new LoginResult
                {
                    CommunicationToken = response.CommunicationToken,
                    EstimatedExpirationTime = response.EstimatedExpirationTime,
                    UserId = response.UserId,
                    UserAvatarUrl = response.ProfilePictureUrl,
                    UserRole = UserRoleConverter.ConvertToLocal(response.UserRole)
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

        public async Task CreateUserAsync(LoginProviderType loginProviderType, UserLoginSkeleton userLoginSkeleton)
        {
            try
            {
                var authenticationProvider = LoginProviderConverter.LoginToAuthenticationProvider(loginProviderType);

                await m_serviceClient.CreateUserAsync(userLoginSkeleton.AccessToken, authenticationProvider, new User
                {
                    Email = userLoginSkeleton.Email,
                    FirstName = userLoginSkeleton.FirstName,
                    LastName = userLoginSkeleton.LastName
                });
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

        public async Task<ObservableCollection<GroupInfoViewModel>> GetGroupListAsync(long userId)
        {
            try {
                var response = await m_serviceClient.GetMembershipsForUserAsync(userId.ToString());
                var list = new ObservableCollection<GroupInfoViewModel>();
                foreach (var groupDetails in response)
                {
                    list.Add(new GroupInfoViewModel
                    {
                        GroupName = groupDetails.Group.Name,
                        MemberCount = groupDetails.Members.Count,
                        GroupId = groupDetails.Id,
                        GroupType = GroupType.Member
                    });
                }
                response = await m_serviceClient.GetGroupsByUserAsync(userId.ToString());
                foreach (var groupDetails in response)
                {
                    list.Add(new GroupInfoViewModel
                    {
                        GroupName = groupDetails.Group.Name,
                        MemberCount = groupDetails.Members.Count,
                        GroupId = groupDetails.Id,
                        GroupType = GroupType.Owner,
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
                var response = await m_serviceClient.CreateGroupAsync(userId.ToString(), groupName);
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
                await m_serviceClient.AddUserToGroupAsync(code, userId.ToString());
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
                var result = await m_serviceClient.GetGroupDetailsAsync(groupId.ToString());
                var group = new GroupInfoViewModel
                {
                    GroupId = result.Id,
                    GroupName = result.Group.Name,
                    MemberCount = result.Members.Count,
                    Members =
                        new ObservableCollection<GroupMemberViewModel>(
                            result.Members.Select(details => new GroupMemberViewModel
                            {
                                FirstName = details.User.FirstName,
                                LastName = details.User.LastName
                            }))
                };
                return group;
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
                var applicationId = ApplicationTypeConverter.ConvertToString(applicationType);
                var synchronizedObject = new SynchronizedObject
                {
                    ObjectType = objectType,
                    Data = objectValue
                };
                await m_serviceClient.CreateSynchronizedObjectAsync(groupId.ToString(), applicationId, userId.ToString(), synchronizedObject);
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

        public async Task<IEnumerable<ObjectDetails>> GetSynchronizedObjectsAsync(ApplicationType applicationType, long groupId, string objectType, DateTime since)
        {
            try
            {
                var applicationId = ApplicationTypeConverter.ConvertToString(applicationType);
                var dateTimeString = since.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
                var objectList = await m_serviceClient.GetSynchronizedObjectsAsync(groupId.ToString(), applicationId, objectType, dateTimeString);
                var outputList = objectList.Select(objectDetails => new ObjectDetails
                {
                    Author = new AuthorInfo
                    {
                        Email = objectDetails.Author.User.Email,
                        FirstName = objectDetails.Author.User.FirstName,
                        LastName = objectDetails.Author.User.LastName,
                        Id = objectDetails.Author.Id
                    },
                    CreateTime = objectDetails.CreateTime,
                    Data = objectDetails.SynchronizedObject.Data,
                    Id = objectDetails.Id
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
