using System.Net;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.RestClient.Errors;
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
                throw new HttpErrorCodeException("FavoriteLabel not found", HttpStatusCode.BadRequest);
            }

            if (label.User.Id != userId)
            {
                throw new HttpErrorCodeException("Current user doesn't own this FavoriteLabel", HttpStatusCode.Forbidden);
            }

            return label;
        }
    }
}