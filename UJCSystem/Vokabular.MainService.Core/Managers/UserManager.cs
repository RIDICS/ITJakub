using System;
using System.Collections.Generic;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.Core.Works.Users;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Results;
using AuthChangeContactContract = Ridics.Authentication.DataContracts.ChangeContactContract;
using AuthChangePasswordContract = Ridics.Authentication.DataContracts.User.ChangePasswordContract;
using AuthChangeTwoFactorContract = Ridics.Authentication.DataContracts.User.ChangeTwoFactorContract;
using AuthConfirmContactContract = Ridics.Authentication.DataContracts.ConfirmContactContract;
using AuthContactContract = Ridics.Authentication.DataContracts.ContactContract;
using AuthUserContract = Ridics.Authentication.DataContracts.User.UserContract;
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
            var user = m_authenticationManager.GetCurrentUser();
            if (user.ExternalId == null)
            {
                throw new ArgumentException($"User with ID {user.Id} has missing ExternalID");
            }

            var client = m_communicationProvider.GetAuthUserApiClient();

            var authUser = client.HttpClient.GetItemAsync<AuthUserContract>(user.ExternalId.Value).GetAwaiter().GetResult();

            authUser.FirstName = data.FirstName;
            authUser.LastName = data.LastName;

            client.EditSelfAsync(user.ExternalId.Value, authUser).GetAwaiter().GetResult();
        }

        public void UpdateUser(int userId, UpdateUserContract data)
        {
            var userExternalId = GetUserExternalId(userId);
            var client = m_communicationProvider.GetAuthUserApiClient();
            var authUser = client.HttpClient.GetItemAsync<AuthUserContract>(userExternalId).GetAwaiter().GetResult();

            authUser.FirstName = data.FirstName;
            authUser.LastName = data.LastName;

            client.HttpClient.EditItemAsync(userExternalId, authUser).GetAwaiter().GetResult();
        }

        public void UpdateUserContact(int userId, UpdateUserContactContract data)
        {
            var contract = Mapper.Map<AuthChangeContactContract>(data);
            contract.UserId = GetUserExternalId(userId);

            var client = m_communicationProvider.GetAuthContactApiClient();
            client.ChangeContactAsync(contract).GetAwaiter().GetResult();
        }

        public void UpdateUserPassword(int userId, UpdateUserPasswordContract data)
        {
            var userExternalId = GetUserExternalId(userId);

            var contract = new AuthChangePasswordContract
            {
                OriginalPassword = data.OldPassword,
                Password = data.NewPassword
            };

            var client = m_communicationProvider.GetAuthUserApiClient();
            client.PasswordChangeAsync(userExternalId, contract).GetAwaiter().GetResult();
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
            var contract = Mapper.Map<AuthConfirmContactContract>(data);
            contract.UserId = GetUserExternalId(userId);

            var client = m_communicationProvider.GetAuthContactApiClient();
            return client.ConfirmContactAsync(contract).GetAwaiter().GetResult();
        }

        public void ResendConfirmCode(int userId, UserContactContract data)
        {
            var contract = Mapper.Map<AuthContactContract>(data);
            contract.UserId = GetUserExternalId(userId);

            var client = m_communicationProvider.GetAuthContactApiClient();
            client.ResendCodeAsync(contract).GetAwaiter().GetResult();
        }

        public void SetTwoFactor(int userId, UpdateTwoFactorContract data)
        {
            var contract = Mapper.Map<AuthChangeTwoFactorContract>(data);

            var client = m_communicationProvider.GetAuthUserApiClient();
            client.SetTwoFactorAsync(GetUserExternalId(userId), contract).GetAwaiter().GetResult();
        }

        public void SelectTwoFactorProvider(int userId, UpdateTwoFactorContract data)
        {
            var contract = Mapper.Map<AuthChangeTwoFactorContract>(data);

            var client = m_communicationProvider.GetAuthUserApiClient();
            client.SelectTwoFactorProviderAsync(GetUserExternalId(userId), contract).GetAwaiter().GetResult();
        }

        private int GetUserExternalId(int userId)
        {
            var user = m_userRepository.InvokeUnitOfWork(x => x.FindById<User>(userId));
            if (user.ExternalId == null)
            {
                throw new ArgumentException($"User with ID {user.Id} has missing ExternalID");
            }

            return user.ExternalId.Value;
        }
    }
}