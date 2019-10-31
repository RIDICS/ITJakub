using System.Collections.Generic;
using System.Linq;
using System.Net;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.Core.Works.Users;
using Vokabular.MainService.DataContracts;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Feedback;
using AuthRoleContractBase = Ridics.Authentication.DataContracts.RoleContractBase;
using AuthUserContract = Ridics.Authentication.DataContracts.User.UserContract;

namespace Vokabular.MainService.Core.Managers
{
    public class UserDetailManager
    {
        private readonly CommunicationProvider m_communicationProvider;
        private readonly UserRepository m_userRepository;
        private readonly IMapper m_mapper;
        private readonly CodeGenerator m_codeGenerator;

        public UserDetailManager(CommunicationProvider communicationProvider, UserRepository userRepository, IMapper mapper, CodeGenerator codeGenerator)
        {
            m_communicationProvider = communicationProvider;
            m_userRepository = userRepository;
            m_mapper = mapper;
            m_codeGenerator = codeGenerator;
        }
        
        public UserContract GetUserContractForUser(UserContract user)
        {
            if (user == null)
                return null;

            var authUser = GetDetailUserFromAuthService(user.ExternalId);
            if (authUser == null)
                return user;

            var userDetailContract = m_mapper.Map<UserContract>(authUser);
            userDetailContract.Id = user.Id;
            return userDetailContract;
        }

        public string GetUserFullName(User user)
        {
            if (user == null)
                return null;

            if (user.ExternalId == null)
            {
                throw new MainServiceException(MainServiceErrorCode.UserHasMissingExternalId,
                    $"User with ID {user.Id} has missing ExternalID",
                    HttpStatusCode.BadRequest,
                    user.Id 
                );
            }

            var authUser = GetDetailUserFromAuthService(user.ExternalId.Value);
            if (authUser == null)
                return string.Empty;

            return $"{authUser.FirstName} {authUser.LastName}";
        }

        public UserDetailContract GetUserDetailContractForUser(User user)
        {
            if (user == null)
                return null;

            if (user.ExternalId == null)
            {
                throw new MainServiceException(MainServiceErrorCode.UserHasMissingExternalId,
                    $"User with ID {user.Id} has missing ExternalID",
                    HttpStatusCode.BadRequest,
                    user.Id
                );
            }

            var authUser = GetDetailUserFromAuthService(user.ExternalId.Value);
            if (authUser == null)
                return null;

            return GetUserDetailContractForUser(authUser, user.Id);
        }

        public UserDetailContract GetUserDetailContractForUser(UserDetailContract user)
        {
            if (user == null)
                return null;

            var authUser = GetDetailUserFromAuthService(user.ExternalId);
            if (authUser == null)
                return user;

            return GetUserDetailContractForUser(authUser, user.Id);
        }

        private UserDetailContract GetUserDetailContractForUser(AuthUserContract authUser, int localUserId)
        {
            var localDbRoles = new GetOrCreateUserGroupsWork<AuthRoleContractBase>(m_userRepository, authUser.Roles).Execute();

            var userDetailContract = m_mapper.Map<UserDetailContract>(authUser);
            userDetailContract.Id = localUserId;
            foreach (var resultRole in userDetailContract.Roles)
            {
                resultRole.Id = localDbRoles.First(x => x.ExternalId == resultRole.ExternalId).Id;
            }

            return userDetailContract;
        }

        public void AddIdForExternalUsers(List<UserDetailContract> userDetailContracts)
        {
            foreach (var userDetailContract in userDetailContracts)
            {
                var userInfo = new UpdateUserInfo(userDetailContract.UserName, userDetailContract.FirstName, userDetailContract.LastName);
                var userId = new CreateOrUpdateUserIfNotExistWork(m_userRepository, userDetailContract.ExternalId, null, userInfo, m_codeGenerator).Execute();
                userDetailContract.Id = userId;
            }
        }

        public void AddIdForExternalUsers(List<UserContract> userDetailContracts)
        {
            foreach (var userDetailContract in userDetailContracts)
            {
                var userInfo = new UpdateUserInfo(userDetailContract.UserName, userDetailContract.FirstName, userDetailContract.LastName);
                var userId = new CreateOrUpdateUserIfNotExistWork(m_userRepository, userDetailContract.ExternalId, null, userInfo, m_codeGenerator).Execute();
                userDetailContract.Id = userId;
            }
        }

        public List<UserContract> AddUserDetails(List<UserContract> list)
        {
            var users = new List<UserContract>();
            foreach (var user in list)
            {
                users.Add(GetUserContractForUser(user));
            }

            return users;
        }

        public List<NewsSyndicationItemContract> AddUserDetails(List<NewsSyndicationItemContract> list)
        {
            foreach (var newsItem in list)
            {
                newsItem.CreatedByUser = GetUserDetailContractForUser(newsItem.CreatedByUser);
            }

            return list;
        }

        public List<FeedbackContract> AddUserDetails(List<FeedbackContract> list)
        {
            foreach (var feedback in list)
            {
                feedback.AuthorUser = GetUserDetailContractForUser(feedback.AuthorUser);
            }

            return list;
        }

        public List<GetTextCommentContract> AddUserDetails(List<GetTextCommentContract> list)
        {
            foreach (var textComment in list)
            {
                AddUserDetails(textComment);
            }

            return list;
        }

        public GetTextCommentContract AddUserDetails(GetTextCommentContract textComment)
        {
            textComment.User = GetUserContractForUser(textComment.User);
            textComment.TextComments = AddUserDetails(textComment.TextComments);
            return textComment;
        }

        private AuthUserContract GetDetailUserFromAuthService(int userExternalId)
        {
            var client = m_communicationProvider.GetAuthUserApiClient();
            var result = client.GetUserAsync(userExternalId).GetAwaiter().GetResult();

            return result;
        }
    }
}