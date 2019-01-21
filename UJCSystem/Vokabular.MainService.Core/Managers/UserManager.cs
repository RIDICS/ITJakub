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

namespace Vokabular.MainService.Core.Managers
{
    public class UserManager
    {
        private readonly UserRepository m_userRepository;
        private readonly CommunicationProvider m_communicationProvider;
        private readonly AuthenticationManager m_authenticationManager;
        private readonly UserDetailManager m_userDetailManager;

        public UserManager(UserRepository userRepository, CommunicationProvider communicationProvider, AuthenticationManager authenticationManager, UserDetailManager userDetailManager)
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

        public void UpdateCurrentUser(UpdateUserContract data)
        {
            var userId = m_authenticationManager.GetCurrentUserId();

            // TODO add data validation

            new UpdateCurrentUserWork(m_userRepository, userId, data, m_communicationProvider).Execute();
        }

        public void UpdateUserPassword(UpdateUserPasswordContract data)
        {
            var userId = m_authenticationManager.GetCurrentUserId();

            // TODO add data validation

            new UpdateUserPasswordWork(m_userRepository, userId, data, m_communicationProvider).Execute();
        }

        public PagedResultList<UserDetailContract> GetUserList(int? start, int? count, string filterByName)
        {        
            var startValue = PagingHelper.GetStart(start);
            var countValue = PagingHelper.GetCount(count);

            using (var client = m_communicationProvider.GetAuthenticationServiceClient())
            {
                var result = client.GetUserAutocomplete(filterByName, startValue, countValue);

                return new PagedResultList<UserDetailContract>
                {
                    List = Mapper.Map<List<UserDetailContract>>(result),
                    TotalCount = result.Count,
                };
            }
        }

        public IList<UserDetailContract> GetUserAutocomplete(string query, int? count)
        {
           if (query == null)
                query = string.Empty;

            var countValue = PagingHelper.GetAutocompleteCount(count);

            using (var client = m_communicationProvider.GetAuthenticationServiceClient())
            {
                var result = client.GetUserAutocomplete(query, null, countValue);
                var userDetailContracts = Mapper.Map<List<UserDetailContract>>(result);
                return m_userDetailManager.GetIdForExternalUsers(userDetailContracts);
            }
        }

        public UserDetailContract GetUserDetail(int userId)
        {
            var dbResult = m_userRepository.InvokeUnitOfWork(x => x.FindById<User>(userId));

            return m_userDetailManager.GetUserDetailContractForUser(dbResult);
        }
    }
}