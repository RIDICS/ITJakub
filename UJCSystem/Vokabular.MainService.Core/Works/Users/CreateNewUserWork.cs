using System;
using Vokabular.Authentication.DataContracts.User;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.Jewelry;
using Vokabular.MainService.Core.Communication;
using CreateUserContract = Vokabular.MainService.DataContracts.Contracts.CreateUserContract;

namespace Vokabular.MainService.Core.Works.Users
{
    public class CreateNewUserWork : UnitOfWorkBase<int>
    {
        private readonly UserRepository m_userRepository;
        private readonly CommunicationProvider m_communicationProvider;
        private readonly CreateUserContract m_data;

        public CreateNewUserWork(UserRepository userRepository, CommunicationProvider communicationProvider, CreateUserContract data) : base(userRepository)
        {
            m_userRepository = userRepository;
            m_communicationProvider = communicationProvider;
            m_data = data;
        }

        protected override int ExecuteWorkImplementation()
        {
            using (var client = m_communicationProvider.GetAuthenticationServiceClient())
            {
                var authUser = new Authentication.DataContracts.User.CreateUserContract
                {
                    Password = m_data.NewPassword,
                    UserName = m_data.UserName,
                    User = new UserContractBase
                    {
                        FirstName = m_data.FirstName,
                        FamilyName = m_data.LastName,
                        Email = m_data.Email,
                        PhoneNumber = "+420739123676"
                    }
                };

                var user = client.CreateUser(authUser);

                var now = DateTime.UtcNow;
                var passwordHash = CustomPasswordHasher.CreateHash(m_data.NewPassword);

                var dbUser = new User
                {
                    ExternalId = user.Id,
                    UserName = m_data.UserName,
                    Email = m_data.Email,
                    FirstName = m_data.FirstName,
                    LastName = m_data.LastName,
                    CreateTime = now,
                    PasswordHash = passwordHash,
                    AvatarUrl = m_data.AvatarUrl,
                    AuthenticationProvider = AuthenticationProvider.ItJakub,
                    CommunicationToken = "example"
                    //Groups = new List<Group> { m_defaultMembershipProvider.GetDefaultRegisteredUserGroup(), m_defaultMembershipProvider.GetDefaultUnRegisteredUserGroup() },
                    //FavoriteLabels = new List<FavoriteLabel> { defaultFavoriteLabel }
                };

                //defaultFavoriteLabel.User = dbUser;
                // TODO generate default FavoriteLabel
                // TODO assign User Groups

                var userId = (int)m_userRepository.Create(dbUser);
                return userId;
            }
        }
    }
}
