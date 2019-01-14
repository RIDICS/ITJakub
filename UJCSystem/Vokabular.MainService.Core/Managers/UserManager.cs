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

        public UserManager(UserRepository userRepository, CommunicationProvider communicationProvider, AuthenticationManager authenticationManager)
        {
            m_userRepository = userRepository;
            m_communicationProvider = communicationProvider;
            m_authenticationManager = authenticationManager;
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

        public List<UserDetailContract> GetUserAutocomplete(string query, int? count)
        {
           if (query == null)
                query = string.Empty;

            var countValue = PagingHelper.GetAutocompleteCount(count);

            using (var client = m_communicationProvider.GetAuthenticationServiceClient())
            {
                var result = client.GetUserAutocomplete(query, null, countValue);
                return Mapper.Map<List<UserDetailContract>>(result);
            }
        }

        public UserDetailContract GetUserDetail(int userId)
        {
            var dbResult = m_userRepository.InvokeUnitOfWork(x => x.FindById<User>(userId));

            using (var client = m_communicationProvider.GetAuthenticationServiceClient())
            {
                var user = client.GetUser(dbResult.ExternalId);

                var result = Mapper.Map<UserDetailContract>(user);
                return result;
            }
        }
    }
}