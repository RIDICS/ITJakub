using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Utils;

namespace Vokabular.MainService.Core.Works.Favorite
{
    public class DeleteFavoriteItemWork : UnitOfWorkBase
    {
        private readonly FavoritesRepository m_favoritesRepository;
        private readonly long m_favoriteId;
        private readonly int m_userId;

        public DeleteFavoriteItemWork(FavoritesRepository favoritesRepository, long favoriteId, int userId) : base(favoritesRepository)
        {
            m_favoritesRepository = favoritesRepository;
            m_favoriteId = favoriteId;
            m_userId = userId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var favoriteItem = m_favoritesRepository.GetFavoriteItem(m_favoriteId);

            OwnershipHelper.CheckItemOwnership(favoriteItem.FavoriteLabel.User.Id, m_userId);

            m_favoritesRepository.Delete(favoriteItem);
        }
    }
}