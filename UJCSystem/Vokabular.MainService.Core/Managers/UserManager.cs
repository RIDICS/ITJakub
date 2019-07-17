using System.Collections.Generic;
using AutoMapper;
using Ridics.Authentication.DataContracts;
using Ridics.Authentication.DataContracts.User;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.Core.Works.Users;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Results;
using AuthUserContract = Ridics.Authentication.DataContracts.User.UserContract;
using CreateUserContract = Vokabular.MainService.DataContracts.Contracts.CreateUserContract;
using UserContactContract = Vokabular.MainService.DataContracts.Contracts.UserContactContract;

namespace Vokabular.MainService.Core.Managers
{
    public class UserManager
    {
        private readonly UserRepository m_userRepository;
        private readonly CommunicationProvider m_communicationProvider;
        private readonly AuthenticationManager m_authenticationManager;
        private readonly UserDetailManager m_userDetailManager;

        public UserManager(UserRepository userRepository, CommunicationProvider communicationProvider,
            AuthenticationManager authenticationManager, UserDetailManager userDetailManager)
        {
            m_userRepository = userRepository;
            m_communicationProvider = communicationProvider;
            m_authenticationManager = authenticationManager;
            m_userDetailManager = userDetailManager;
        }

        public int CreateNewUser(CreateUserContract data)
        {
            var userId = new CreateNewUserWork(m_userRepository, m_communicationProvider, data).Execute();
            return userId;
        }

        public int CreateUserIfNotExist(int externalId)
        {
            var authUserApiClient = m_communicationProvider.GetAuthUserApiClient();
            var userRoles = authUserApiClient.GetRolesByUserAsync(externalId).GetAwaiter().GetResult();
            var userId = new CreateUserIfNotExistWork(m_userRepository, externalId, userRoles).Execute();
            return userId;
        }

        public void UpdateCurrentUser(UpdateUserContract data)
        {
            var userId = m_authenticationManager.GetCurrentUserId();

            new UpdateCurrentUserWork(m_userRepository, userId, data, m_communicationProvider).Execute();
        }

        public void UpdateUser(int userId, UpdateUserContract data)
        {
            new UpdateUserWork(m_userRepository, userId, data, m_communicationProvider).Execute();
        }

        public void UpdateUserContact(int userId, UpdateUserContactContract data)
        {
            new UpdateUserContactsWork(m_userRepository, userId, data, m_communicationProvider).Execute();
        }

        public void UpdateUserPassword(int userId, UpdateUserPasswordContract data)
        {
            new UpdateUserPasswordWork(m_userRepository, userId, data, m_communicationProvider).Execute();
        }

        public PagedResultList<UserDetailContract> GetUserList(int? start, int? count, string filterByName)
        {
            var startValue = PagingHelper.GetStart(start);
            var countValue = PagingHelper.GetCount(count);

            var client = m_communicationProvider.GetAuthUserApiClient();

            var result = client.HttpClient.GetListAsync<AuthUserContract>(startValue, countValue, filterByName).GetAwaiter().GetResult();
            var userDetailContracts = Mapper.Map<List<UserDetailContract>>(result.Items);
            m_userDetailManager.AddIdForExternalUsers(userDetailContracts);

            return new PagedResultList<UserDetailContract>
            {
                List = userDetailContracts,
                TotalCount = result.ItemsCount,
            };
        }

        public IList<UserDetailContract> GetUserAutocomplete(string query, int? count)
        {
            if (query == null)
                query = string.Empty;

            var countValue = PagingHelper.GetAutocompleteCount(count);

            var client = m_communicationProvider.GetAuthUserApiClient();

            var result = client.HttpClient.GetListAsync<AuthUserContract>(0, countValue, query).GetAwaiter().GetResult();
            var userDetailContracts = Mapper.Map<List<UserDetailContract>>(result.Items);
            m_userDetailManager.AddIdForExternalUsers(userDetailContracts);
            return userDetailContracts;
        }

        public UserDetailContract GetUserDetail(int userId)
        {
            var dbResult = m_userRepository.InvokeUnitOfWork(x => x.FindById<User>(userId));

            return m_userDetailManager.GetUserDetailContractForUser(dbResult);
        }

        public bool ConfirmContact(int userId, ConfirmUserContactContract data)
        {
            var client = m_communicationProvider.GetAuthContactApiClient();

            var contract = new ConfirmContactContract
            {
                UserId = userId,
                ConfirmCode = data.ConfirmCode,
                ContactType = data.ContactType
            };

            return client.ConfirmContactAsync(contract).GetAwaiter().GetResult();
        }

        public void ResendConfirmCode(int userId, UserContactContract data)
        {
            var client = m_communicationProvider.GetAuthContactApiClient();

            var contract = new ContactContract
            {
                UserId = userId,
                ContactType = data.ContactType
            };

            client.ResendCodeAsync(contract).GetAwaiter().GetResult();
        }
    }
}