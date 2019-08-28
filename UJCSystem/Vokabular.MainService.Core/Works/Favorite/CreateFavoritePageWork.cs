using System;
using System.Net;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts;
using Vokabular.MainService.DataContracts.Contracts.Favorite;

namespace Vokabular.MainService.Core.Works.Favorite
{
    public class CreateFavoritePageWork : CreateFavoriteItemWorkBase
    {
        private readonly FavoritesRepository m_favoritesRepository;
        private readonly ResourceRepository m_resourceRepository;
        private readonly CreateFavoritePageContract m_data;
        private readonly int m_userId;

        public CreateFavoritePageWork(FavoritesRepository favoritesRepository, ResourceRepository resourceRepository, CreateFavoritePageContract data, int userId) : base(favoritesRepository)
        {
            m_favoritesRepository = favoritesRepository;
            m_resourceRepository = resourceRepository;
            m_data = data;
            m_userId = userId;
        }

        protected override long ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;
            var user = m_favoritesRepository.Load<User>(m_userId);
            var bookPage = m_resourceRepository.GetLatestResourceVersion<PageResource>(m_data.PageId);

            if (bookPage == null)
            {
                throw new MainServiceException(MainServiceErrorCode.PageNotFound, 
                    $"Page with ID {m_data.PageId} not found",
                    HttpStatusCode.NotFound,
                    new object[] { m_data.PageId });
            }

            var label = GetFavoriteLabelAndCheckAuthorization(m_data.FavoriteLabelId, user.Id);
            label.LastUseTime = now;

            var favoriteItem = new FavoritePage
            {
                ResourcePage = bookPage.Resource,
                Title = m_data.Title,
                FavoriteLabel = label,
                CreateTime = now,
            };

            var resultId = (long) m_favoritesRepository.Create(favoriteItem);
            return resultId;
        }
    }
}
