using System;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Users
{
    public class CreateUserIfNotExistWork : UnitOfWorkBase<int>
    {
        private readonly UserRepository m_userRepository;
        private readonly int m_userExternalId;

        public CreateUserIfNotExistWork(UserRepository userRepository, int userExternalId) : base(userRepository)
        {
            m_userRepository = userRepository;
            m_userExternalId = userExternalId;
        }

        protected override int ExecuteWorkImplementation()
        {
            var user = m_userRepository.GetUserByExternalId(m_userExternalId);
            if (user != null)
            {
                return user.Id;
            }
           
            var now = DateTime.UtcNow;

            var dbUser = new User
            {
                ExternalId = m_userExternalId,
                CreateTime = now,
                AvatarUrl = null
                //Groups = new List<Group> { m_defaultMembershipProvider.GetDefaultRegisteredUserGroup(), m_defaultMembershipProvider.GetDefaultUnRegisteredUserGroup() },
                //FavoriteLabels = new List<FavoriteLabel> { defaultFavoriteLabel }
            };

            //defaultFavoriteLabel.User = dbUser;
            // TODO generate default FavoriteLabel
            // TODO assign User Groups

            var userId = (int) m_userRepository.Create(dbUser);
            return userId;
        }
    }
}