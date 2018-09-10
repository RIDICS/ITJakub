using System.Collections.Generic;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Managers.Authentication;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.Core.Works.Users;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Results;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Managers
{
    public class UserManager
    {
        private readonly UserRepository m_userRepository;
        private readonly ICommunicationTokenGenerator m_communicationTokenGenerator;
        private readonly AuthorizationManager m_authorizationManager;
        private readonly AuthenticationManager m_authenticationManager;

        public UserManager(UserRepository userRepository, ICommunicationTokenGenerator communicationTokenGenerator, AuthorizationManager authorizationManager, AuthenticationManager authenticationManager)
        {
            m_userRepository = userRepository;
            m_communicationTokenGenerator = communicationTokenGenerator;
            m_authorizationManager = authorizationManager;
            m_authenticationManager = authenticationManager;
        }

        public int CreateNewUser(CreateUserContract data)
        {
            var userId = new CreateNewUserWork(m_userRepository, m_communicationTokenGenerator, data).Execute();
            return userId;
        }

        public UserDetailContract GetCurrentUserByToken()
        {
            var dbUser = m_authenticationManager.GetCurrentUser(false);
            var result = Mapper.Map<UserDetailContract>(dbUser);
            return result;
        }

        public void UpdateUser(UpdateUserContract data)
        {
            var userId = m_authenticationManager.GetCurrentUserId();

            // TODO add data validation

            new UpdateUserWork(m_userRepository, userId, data).Execute();
        }

        public void UpdateUserPassword(UpdateUserPasswordContract data)
        {
            var userId = m_authenticationManager.GetCurrentUserId();

            // TODO add data validation

            new UpdateUserPasswordWork(m_userRepository, userId, data).Execute();
        }

        public PagedResultList<UserDetailContract> GetUserList(int? start, int? count, string filterByName)
        {
            m_authorizationManager.CheckUserCanManagePermissions();

            var startValue = PagingHelper.GetStart(start);
            var countValue = PagingHelper.GetCount(count);

            var dbResult = m_userRepository.InvokeUnitOfWork(x => x.GetUserList(startValue, countValue, filterByName));
            return new PagedResultList<UserDetailContract>
            {
                List = Mapper.Map<List<UserDetailContract>>(dbResult.List),
                TotalCount = dbResult.Count,
            };
        }

        public List<UserDetailContract> GetUserAutocomplete(string query, int? count)
        {
            m_authorizationManager.CheckUserCanManagePermissions();

            if (query == null)
                query = string.Empty;

            var countValue = PagingHelper.GetAutocompleteCount(count);

            var result = m_userRepository.InvokeUnitOfWork(x => x.GetUserAutocomplete(query, countValue));
            return Mapper.Map<List<UserDetailContract>>(result);
        }

        public UserDetailContract GetUserDetail(int userId)
        {
            m_authorizationManager.CheckUserCanManagePermissions();

            var dbResult = m_userRepository.InvokeUnitOfWork(x => x.FindById<User>(userId));
            var result = Mapper.Map<UserDetailContract>(dbResult);

            return result;
        }
    }
}