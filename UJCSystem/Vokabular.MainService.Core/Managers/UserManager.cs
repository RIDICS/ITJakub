﻿using System.Collections.Generic;
using System.Net;
using AutoMapper;
using Microsoft.Extensions.Options;
using Ridics.Authentication.DataContracts;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.Core.Works.Users;
using Vokabular.MainService.DataContracts;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Results;
using Vokabular.Shared.DataEntities.UnitOfWork;
using Vokabular.Shared.Options;
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
        private readonly IMapper m_mapper;
        private readonly CodeGenerator m_codeGenerator;
        private readonly RegistrationOption m_registrationOption;

        public UserManager(UserRepository userRepository, CommunicationProvider communicationProvider,
            AuthenticationManager authenticationManager, UserDetailManager userDetailManager, IMapper mapper,
            CodeGenerator codeGenerator, IOptions<RegistrationOption> registrationOption)
        {
            m_userRepository = userRepository;
            m_communicationProvider = communicationProvider;
            m_authenticationManager = authenticationManager;
            m_userDetailManager = userDetailManager;
            m_mapper = mapper;
            m_codeGenerator = codeGenerator;
            m_registrationOption = registrationOption.Value;
        }

        public int CreateNewUser(CreateUserContract data)
        {
            if (m_registrationOption.ReservedUsernames.Contains(data.UserName.ToLower()))
            {
                throw new MainServiceException(MainServiceErrorCode.ReservedUsernameError, $"Username '{data.UserName}' is reserved, cannot be used.", HttpStatusCode.BadRequest, data.UserName);
            }

            var userId = new CreateNewUserWork(m_userRepository, m_communicationProvider, data, m_codeGenerator).Execute();
            return userId;
        }

        public int CreateUserIfNotExist(CreateUserIfNotExistContract data)
        {
            var userExternalId = data.ExternalId;
            var userInfo = new UpdateUserInfo(data.Username, data.FirstName, data.LastName);

            var authUserApiClient = m_communicationProvider.GetAuthUserApiClient();
            var userRoles = authUserApiClient.GetRolesByUserAsync(userExternalId).GetAwaiter().GetResult();
            var userId = new CreateOrUpdateUserIfNotExistWork(m_userRepository, userExternalId, userRoles, userInfo, m_codeGenerator).Execute();
            return userId;
        }

        public void UpdateCurrentUser(UpdateUserContract data)
        {
            var user = m_authenticationManager.GetCurrentUser();
            if (user.ExternalId == null)
            {
                throw new MainServiceException(MainServiceErrorCode.UserHasMissingExternalId,
                    $"User with ID {user.Id} has missing ExternalID",
                    HttpStatusCode.BadRequest,
                    new object[] { user.Id }
                );
            }

            var client = m_communicationProvider.GetAuthUserApiClient();

            var authUser = client.GetUserAsync(user.ExternalId.Value).GetAwaiter().GetResult();

            authUser.FirstName = data.FirstName;
            authUser.LastName = data.LastName;

            client.EditSelfAsync(user.ExternalId.Value, authUser).GetAwaiter().GetResult();

            var updateUserInfo = new UpdateUserInfo(authUser.UserName, authUser.FirstName, authUser.LastName);
            new UpdateUserWork(m_userRepository, user.Id, updateUserInfo).Execute();
        }

        public void UpdateUser(int userId, UpdateUserContract data)
        {
            var userExternalId = GetUserExternalId(userId);
            var client = m_communicationProvider.GetAuthUserApiClient();
            var authUser = client.GetUserAsync(userExternalId).GetAwaiter().GetResult();

            authUser.FirstName = data.FirstName;
            authUser.LastName = data.LastName;

            client.EditUserAsync(userExternalId, authUser).GetAwaiter().GetResult();

            var updateUserInfo = new UpdateUserInfo(authUser.UserName, authUser.FirstName, authUser.LastName);
            new UpdateUserWork(m_userRepository, userId, updateUserInfo).Execute();
        }

        public void UpdateUserContact(int userId, UpdateUserContactContract data)
        {
            var userExternalId = GetUserExternalId(userId);

            var contract = new AuthChangeContactContract
            {
                ContactType = m_mapper.Map<ContactTypeEnum>(data.ContactType),
                UserId = userExternalId,
                NewContactValue = data.NewContactValue
            };

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

            var result = client.GetUserListAsync(startValue, countValue, filterByName).GetAwaiter().GetResult();
            var userDetailContracts = m_mapper.Map<List<UserDetailContract>>(result.Items);
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

            var result = client.GetUserListAsync(0, countValue, query).GetAwaiter().GetResult();
            var userDetailContracts = m_mapper.Map<List<UserDetailContract>>(result.Items);
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
            var contract = new AuthConfirmContactContract
            {
                UserId = GetUserExternalId(userId),
                ConfirmCode = data.ConfirmCode,
                ContactType = m_mapper.Map<ContactTypeEnum>(data.ContactType),
            };

            var client = m_communicationProvider.GetAuthContactApiClient();
            return client.ConfirmContactAsync(contract).GetAwaiter().GetResult();
        }

        public void ResendConfirmCode(int userId, UserContactContract data)
        {
            var contract = new AuthContactContract
            {
                UserId = GetUserExternalId(userId),
                ContactType = m_mapper.Map<ContactTypeEnum>(data.ContactType),
            };

            var client = m_communicationProvider.GetAuthContactApiClient();
            client.ResendCodeAsync(contract).GetAwaiter().GetResult();
        }

        public void SetTwoFactor(int userId, UpdateTwoFactorContract data)
        {
            var contract = new AuthChangeTwoFactorContract
            {
                TwoFactorIsEnabled = data.TwoFactorIsEnabled,
            };

            var client = m_communicationProvider.GetAuthUserApiClient();
            client.SetTwoFactorAsync(GetUserExternalId(userId), contract).GetAwaiter().GetResult();
        }

        public void SelectTwoFactorProvider(int userId, UpdateTwoFactorProviderContract data)
        {
            var contract = new AuthChangeTwoFactorContract
            {
                TwoFactorProvider = data.TwoFactorProvider,
            };

            var client = m_communicationProvider.GetAuthUserApiClient();
            client.SelectTwoFactorProviderAsync(GetUserExternalId(userId), contract).GetAwaiter().GetResult();
        }

        private int GetUserExternalId(int userId)
        {
            var user = m_userRepository.InvokeUnitOfWork(x => x.FindById<User>(userId));
            if (user.ExternalId == null)
            {
                throw new MainServiceException(MainServiceErrorCode.UserHasMissingExternalId,
                    $"User with ID {user.Id} has missing ExternalID",
                    HttpStatusCode.BadRequest,
                    new object[] { user.Id }
                );
            }

            return user.ExternalId.Value;
        }

        public void ResetUserPassword(int userId)
        {
            var client = m_communicationProvider.GetAuthUserApiClient();
            client.ResetUserPasswordAsync(GetUserExternalId(userId)).GetAwaiter().GetResult();
        }
    }
}