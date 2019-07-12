using System;
using System.Collections.Generic;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.Core.Works.Users;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Feedback;

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

        public UserDetailContract GetUserDetailContractForUser(User user)
        {
            if (user.ExternalId == null)
            {
                throw new ArgumentException($"User with ID {user.Id} has missing ExternalID");
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

        public List<UserDetailContract> GetIdForExternalUsers(List<UserDetailContract> userDetailContracts)
        {
            foreach (var userDetailContract in userDetailContracts)
            {
                var userId = new CreateUserIfNotExistWork(m_userRepository, userDetailContract.ExternalId, null).Execute();
                userDetailContract.Id = userId;
            }

            return userDetailContracts;
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

        private Ridics.Authentication.DataContracts.User.UserContract GetDetailUserFromAuthService(int userExternalId)
        {
            var client = m_communicationProvider.GetAuthUserApiClient();
            var result = client.HttpClient.GetItemAsync<Ridics.Authentication.DataContracts.User.UserContract>(userExternalId).GetAwaiter().GetResult();
            return result;
        }
    }
}