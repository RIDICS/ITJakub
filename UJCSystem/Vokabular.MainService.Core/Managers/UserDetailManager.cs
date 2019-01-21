﻿using System;
using System.Collections.Generic;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Feedback;

namespace Vokabular.MainService.Core.Managers
{
    public class UserDetailManager
    {
        private readonly CommunicationProvider m_communicationProvider;
        private readonly UserRepository m_userRepository;

        public UserDetailManager(CommunicationProvider communicationProvider, UserRepository m_userRepository)
        {
            m_communicationProvider = communicationProvider;
            this.m_userRepository = m_userRepository;
        }

        public UserContract GetUserContractForUser(User user)
        {
            var authUser = GetDetailForUser(user.ExternalId);
            if (authUser == null)
                return null;

            var userDetailContract = Mapper.Map<UserContract>(authUser);
            userDetailContract.Id = user.Id;
            userDetailContract.AvatarUrl = user.AvatarUrl;
            return userDetailContract;
        }

        public UserContract GetUserContractForUser(UserContract user)
        {
            var authUser = GetDetailForUser(user.ExternalId);
            if (authUser == null)
                return user;

            var userDetailContract = Mapper.Map<UserContract>(authUser);
            userDetailContract.Id = user.Id;
            userDetailContract.AvatarUrl = user.AvatarUrl;
            return userDetailContract;
        }

        public UserDetailContract GetUserDetailContractForUser(User user)
        {
            var authUser = GetDetailForUser(user.ExternalId);
            if (authUser == null)
                return null;

            var userDetailContract = Mapper.Map<UserDetailContract>(authUser);
            userDetailContract.Id = user.Id;
            userDetailContract.AvatarUrl = user.AvatarUrl;
            return userDetailContract;
        }

        public UserDetailContract GetUserDetailContractForUser(UserDetailContract user)
        {
            var authUser = GetDetailForUser(user.ExternalId);
            if (authUser == null)
                return user;

            var userDetailContract = Mapper.Map<UserDetailContract>(authUser);
            userDetailContract.Id = user.Id;
            userDetailContract.AvatarUrl = user.AvatarUrl;
            return userDetailContract;
        }

        public List<UserDetailContract> GetIdForExternalUsers(List<UserDetailContract> userDetailContracts)
        {
            foreach (var userDetailContract in userDetailContracts)
            {
                var user = m_userRepository.InvokeUnitOfWork(x => x.GetUserByExternalId(userDetailContract.ExternalId));
                if (user != null)
                {
                    userDetailContract.AvatarUrl = user.AvatarUrl;
                    userDetailContract.Id = user.Id;
                }
            }

            return userDetailContracts;
        }

        public List<UserContract> GetUserDetailContracts(List<UserContract> list)
        {
            var users = new List<UserContract>();
            foreach (var user in list)
            {
                users.Add(GetUserContractForUser(user));
            }

            return users;
        }

        public List<NewsSyndicationItemContract> GetUserDetailContracts(List<NewsSyndicationItemContract> list)
        {
            foreach (var newsItem in list)
            {
                newsItem.CreatedByUser = GetUserDetailContractForUser(newsItem.CreatedByUser);
            }

            return list;
        }

        public List<FeedbackContract> GetUserDetailContracts(List<FeedbackContract> list)
        {
            foreach (var feedback in list)
            {
                feedback.AuthorUser = GetUserDetailContractForUser(feedback.AuthorUser);
            }

            return list;
        }

        private Vokabular.Authentication.DataContracts.User.UserContract GetDetailForUser(int userId)
        {
            using (var client = m_communicationProvider.GetAuthenticationServiceClient())
            {
                return client.GetUser(userId);
            }
        }
    }
}