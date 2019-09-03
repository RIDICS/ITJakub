using System.Collections.Generic;
using System.Net;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.Core.Works.Users;
using Vokabular.MainService.DataContracts;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Feedback;
using AuthUserContract = Ridics.Authentication.DataContracts.User.UserContract;

namespace Vokabular.MainService.Core.Managers
{
    public class UserDetailManager
    {
        private readonly CommunicationProvider m_communicationProvider;
        private readonly UserRepository m_userRepository;

        public UserDetailManager(CommunicationProvider communicationProvider, UserRepository userRepository)
        {
            m_communicationProvider = communicationProvider;
            m_userRepository = userRepository;
        }
        
        public UserContract GetUserContractForUser(UserContract user)
        {
            var authUser = GetDetailUserFromAuthService(user.ExternalId);
            if (authUser == null)
                return user;

            var userDetailContract = Mapper.Map<UserContract>(authUser);
            userDetailContract.Id = user.Id;
            return userDetailContract;
        }

        public string GetUserName(User user)
        {
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

            var userDetailContract = Mapper.Map<UserDetailContract>(authUser);
            userDetailContract.Id = user.Id;
            return userDetailContract;
        }

        public UserDetailContract GetUserDetailContractForUser(UserDetailContract user)
        {
            var authUser = GetDetailUserFromAuthService(user.ExternalId);
            if (authUser == null)
                return user;

            var userDetailContract = Mapper.Map<UserDetailContract>(authUser);
            userDetailContract.Id = user.Id;
            return userDetailContract;
        }

        public void AddIdForExternalUsers(List<UserDetailContract> userDetailContracts)
        {
            foreach (var userDetailContract in userDetailContracts)
            {
                var userId = new CreateUserIfNotExistWork(m_userRepository, userDetailContract.ExternalId, null).Execute();
                userDetailContract.Id = userId;
            }
        }

        public void AddIdForExternalUsers(List<UserContract> userDetailContracts)
        {
            foreach (var userDetailContract in userDetailContracts)
            {
                var userId = new CreateUserIfNotExistWork(m_userRepository, userDetailContract.ExternalId, null).Execute();
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
                textComment.User = GetUserContractForUser(textComment.User);
                textComment.TextComments = AddUserDetails(textComment.TextComments);
            }

            return list;
        }

        private AuthUserContract GetDetailUserFromAuthService(int userExternalId)
        {
            var client = m_communicationProvider.GetAuthUserApiClient();
            var result = client.HttpClient.GetItemAsync<AuthUserContract>(userExternalId).GetAwaiter().GetResult();

            return result;
        }
    }
}