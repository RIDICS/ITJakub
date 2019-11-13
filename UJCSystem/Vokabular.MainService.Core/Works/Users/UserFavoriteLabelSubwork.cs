using System;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;

namespace Vokabular.MainService.Core.Works.Users
{
    public class UserFavoriteLabelSubwork
    {
        private const string DefaultName = "Výchozí";
        private const string DefaultColor = "#EEB711";

        private readonly UserRepository m_userRepository;

        public UserFavoriteLabelSubwork(UserRepository userRepository)
        {
            m_userRepository = userRepository;
        }

        public FavoriteLabel GetNewDefaultFavoriteLabel(User user)
        {
            var now = DateTime.UtcNow;
            var result = new FavoriteLabel
            {
                Name = DefaultName,
                Color = DefaultColor,
                IsDefault = true,
                LastUseTime = now,
                User = user,
                FavoriteItems = null,
            };

            return result;
        }

        public FavoriteLabel CreateOrUpdateDefaultFavoriteLabel(int userId)
        {
            var defaultFavoriteLabel = m_userRepository.GetDefaultFavoriteLabelForUser(userId);

            if (defaultFavoriteLabel != null)
            {
                // It is possible to update Favorite label data here if required

                return defaultFavoriteLabel;
            }

            var user = m_userRepository.Load<User>(userId);
            var newDefaultLabel = GetNewDefaultFavoriteLabel(user);
            
            m_userRepository.Create(newDefaultLabel);

            return newDefaultLabel;
        }
    }
}
