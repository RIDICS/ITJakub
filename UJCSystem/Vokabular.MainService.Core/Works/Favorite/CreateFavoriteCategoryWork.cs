using System;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts.Contracts.Favorite;

namespace Vokabular.MainService.Core.Works.Favorite
{
    public class CreateFavoriteCategoryWork : CreateFavoriteItemWorkBase
    {
        private readonly FavoritesRepository m_favoritesRepository;
        private readonly CreateFavoriteCategoryContract m_data;
        private readonly int m_userId;

        public CreateFavoriteCategoryWork(FavoritesRepository favoritesRepository, CreateFavoriteCategoryContract data, int userId) : base(favoritesRepository)
        {
            m_favoritesRepository = favoritesRepository;
            m_data = data;
            m_userId = userId;
        }

        protected override long ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;
            var user = m_favoritesRepository.Load<User>(m_userId);
            var category = m_favoritesRepository.Load<Category>(m_data.CategoryId);

            var label = GetFavoriteLabelAndCheckAuthorization(m_data.FavoriteLabelId, user.Id);

            label.LastUseTime = now;

            var favoriteItem = new FavoriteCategory
            {
                Category = category,
                CreateTime = now,
                FavoriteLabel = label,
                Title = m_data.Title
            };

            var resultId = (long)m_favoritesRepository.Create(favoriteItem);
            return resultId;
        }
    }
}