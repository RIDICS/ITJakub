using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Favorite
{
    public abstract class CreateFavoriteItemWorkBase : UnitOfWorkBase<long>
    {
        private readonly FavoritesRepository m_favoritesRepository;

        protected CreateFavoriteItemWorkBase(FavoritesRepository favoritesRepository) : base(favoritesRepository)
        {
            m_favoritesRepository = favoritesRepository;
        }

        protected FavoriteLabel GetFavoriteLabelAndCheckAuthorization(long? labelId, int userId)
        {
            if (labelId == null)
            {
                var defaultLabel = m_favoritesRepository.GetDefaultFavoriteLabel(userId);
                return defaultLabel;
            }

            var label = m_favoritesRepository.FindById<FavoriteLabel>(labelId.Value);

            if (label == null)
            {
                throw new MainServiceException(MainServiceErrorCode.FavoriteLabelNotFound, "FavoriteLabel not found");
            }

            if (label.User.Id != userId)
            {
                throw new MainServiceException(MainServiceErrorCode.UserDoesNotOwnLabel, "Current user doesn't own this FavoriteLabel");
            }

            return label;
        }
    }
}