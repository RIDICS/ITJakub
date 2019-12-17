using System;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts.Contracts.Favorite;

namespace Vokabular.MainService.Core.Works.Favorite
{
    public class CreateFavoriteHeadwordWork : CreateFavoriteItemWorkBase
    {
        private readonly FavoritesRepository m_favoritesRepository;
        private readonly CreateFavoriteHeadwordContract m_data;
        private readonly int m_userId;

        public CreateFavoriteHeadwordWork(FavoritesRepository favoritesRepository, CreateFavoriteHeadwordContract data, int userId) : base(favoritesRepository)
        {
            m_favoritesRepository = favoritesRepository;
            m_data = data;
            m_userId = userId;
        }

        protected override long ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;
            var headword = m_favoritesRepository.Load<Resource>(m_data.HeadwordId);
            var label = m_favoritesRepository.GetDefaultFavoriteLabel(m_userId);

            label.LastUseTime = now;

            var favoriteItem = new FavoriteHeadword
            {
                CreateTime = now,
                FavoriteLabel = label,
                Title = m_data.Title,
                DefaultHeadwordResource = headword,
                Query = m_data.Title,
            };

            var resultId = (long)m_favoritesRepository.Create(favoriteItem);
            return resultId;
        }
    }
}