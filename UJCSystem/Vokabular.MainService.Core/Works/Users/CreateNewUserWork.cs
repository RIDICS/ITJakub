using System;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.Jewelry;
using Vokabular.MainService.Core.Managers.Authentication;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Users
{
    public class CreateNewUserWork : UnitOfWorkBase<int>
    {
        private readonly UserRepository m_userRepository;
        private readonly ICommunicationTokenGenerator m_communicationTokenGenerator;
        private readonly CreateUserContract m_data;

        public CreateNewUserWork(UserRepository userRepository, ICommunicationTokenGenerator communicationTokenGenerator, CreateUserContract data) : base(userRepository)
        {
            m_userRepository = userRepository;
            m_communicationTokenGenerator = communicationTokenGenerator;
            m_data = data;
        }

        protected override int ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;
            var passwordHash = CustomPasswordHasher.CreateHash(m_data.NewPassword);
            
            var dbUser = new User
            {
                UserName = m_data.UserName,
                Email = m_data.Email,
                FirstName = m_data.FirstName,
                LastName = m_data.LastName,
                CreateTime = now,
                PasswordHash = passwordHash,
                AvatarUrl = m_data.AvatarUrl,
                AuthenticationProvider = AuthenticationProvider.ItJakub,
                CommunicationToken = null,
                CommunicationTokenCreateTime = null,
                //Groups = new List<Group> { m_defaultMembershipProvider.GetDefaultRegisteredUserGroup(), m_defaultMembershipProvider.GetDefaultUnRegisteredUserGroup() },
                //FavoriteLabels = new List<FavoriteLabel> { defaultFavoriteLabel }
            };

            dbUser.CommunicationToken = m_communicationTokenGenerator.GetNewCommunicationToken(dbUser);
            dbUser.CommunicationTokenCreateTime = now;
            
            //defaultFavoriteLabel.User = dbUser;
            // TODO generate default FavoriteLabel
            // TODO assign User Groups

            var userId = (int) m_userRepository.Create(dbUser);
            return userId;
        }
    }
}
