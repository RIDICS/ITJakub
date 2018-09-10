using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.DataContracts.Contracts.Favorite;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Favorite
{
    public class UpdateFavoriteItemWork : UnitOfWorkBase
    {
        private readonly FavoritesRepository m_favoritesRepository;
        private readonly long m_favoriteId;
        private readonly UpdateFavoriteContract m_data;
        private readonly int m_userId;

        public UpdateFavoriteItemWork(FavoritesRepository favoritesRepository, long favoriteId, UpdateFavoriteContract data, int userId) : base(favoritesRepository)
        {
            m_favoritesRepository = favoritesRepository;
            m_favoriteId = favoriteId;
            m_data = data;
            m_userId = userId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var favoriteItem = m_favoritesRepository.GetFavoriteItem(m_favoriteId);

            OwnershipHelper.CheckItemOwnership(favoriteItem.FavoriteLabel.User.Id, m_userId);

            favoriteItem.Title = m_data.Name;

            m_favoritesRepository.Update(favoriteItem);
        }
    }
}