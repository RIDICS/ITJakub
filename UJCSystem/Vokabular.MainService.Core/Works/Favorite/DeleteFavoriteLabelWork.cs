using System.Net;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Utils;
using Vokabular.RestClient.Errors;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Favorite
{
    public class DeleteFavoriteLabelWork : UnitOfWorkBase
    {
        private readonly FavoritesRepository m_favoritesRepository;
        private readonly long m_favoriteLabelId;
        private readonly int m_userId;

        public DeleteFavoriteLabelWork(FavoritesRepository favoritesRepository, long favoriteLabelId, int userId) : base(favoritesRepository)
        {
            m_favoritesRepository = favoritesRepository;
            m_favoriteLabelId = favoriteLabelId;
            m_userId = userId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var favoriteLabel = m_favoritesRepository.FindById<FavoriteLabel>(m_favoriteLabelId);

            OwnershipHelper.CheckItemOwnership(favoriteLabel.User.Id, m_userId);

            if (favoriteLabel.IsDefault)
                throw new HttpErrorCodeException("Can't remove default favorite label", HttpStatusCode.BadRequest);

            m_favoritesRepository.Delete(favoriteLabel);
        }
    }
}