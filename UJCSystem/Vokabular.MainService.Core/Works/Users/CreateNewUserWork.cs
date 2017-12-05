using System;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.Jewelry;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.Works.Users
{
    public class CreateNewUserWork : UnitOfWorkBase<int>
    {
        private readonly UserRepository m_userRepository;
        private readonly CreateUserContract m_data;

        public CreateNewUserWork(UserRepository userRepository, CreateUserContract data) : base(userRepository)
        {
            m_userRepository = userRepository;
            m_data = data;
        }

        protected override int ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;
            var passwordHash = CustomPasswordHasher.CreateHash(m_data.NewPassword);
            
            var dbUser = new DataEntities.Database.Entities.User
            {
                UserName = m_data.UserName,
                Email = m_data.Email,
                FirstName = m_data.FirstName,
                LastName = m_data.LastName,
                CreateTime = now,
                PasswordHash = passwordHash,
                AvatarUrl = m_data.AvatarUrl,
                AuthenticationProvider = AuthenticationProvider.ItJakub,
                //CommunicationToken = m_communicationTokenGenerator.GetNewCommunicationToken(),
                CommunicationTokenCreateTime = now,
                //Groups = new List<Group> { m_defaultMembershipProvider.GetDefaultRegisteredUserGroup(), m_defaultMembershipProvider.GetDefaultUnRegisteredUserGroup() },
                //FavoriteLabels = new List<FavoriteLabel> { defaultFavoriteLabel }
            };
            //defaultFavoriteLabel.User = dbUser;
            // TODO generate CommunicationToken - probably second step (Login)
            // TODO generate default FavoriteLabel
            // TODO assign User Groups

            var userId = (int) m_userRepository.Create(dbUser);
            return userId;
        }
    }
}
