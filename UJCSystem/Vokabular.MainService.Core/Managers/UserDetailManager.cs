using Vokabular.DataEntities.Database.Entities;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.Managers
{
    public class UserDetailManager
    {
        private readonly CommunicationProvider m_communicationProvider;

        public UserDetailManager(CommunicationProvider communicationProvider)
        {
            m_communicationProvider = communicationProvider;
        }

        public UserContract GetUserContractForUser(User user)
        {
            var authUser = GetDetailForUser(user.ExternalId);
            return new UserContract
            {
                FirstName = authUser.FirstName,
                LastName = authUser.FamilyName,
                Id = user.Id,
                UserName = authUser.UserName,
                AvatarUrl = user.AvatarUrl
            };
        }

        public UserDetailContract GetUserDetailContractForUser(User user)
        {
            var authUser = GetDetailForUser(user.ExternalId);
            return new UserDetailContract
            {
                FirstName = authUser.FirstName,
                LastName = authUser.FamilyName,
                Id = user.Id,
                UserName = authUser.UserName,
                AvatarUrl = user.AvatarUrl,
                Email = authUser.Email
            };
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