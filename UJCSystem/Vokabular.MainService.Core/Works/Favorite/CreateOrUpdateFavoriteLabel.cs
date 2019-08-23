using System;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.DataContracts;
using Vokabular.MainService.DataContracts.Contracts.Favorite;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Favorite
{
    public class CreateOrUpdateFavoriteLabelWork : UnitOfWorkBase<long>
    {
        private readonly FavoritesRepository m_favoritesRepository;
        private readonly FavoriteLabelContractBase m_data;
        private readonly int m_userId;
        private readonly long? m_favoriteLabelId;
        private readonly bool m_createDefault;

        public CreateOrUpdateFavoriteLabelWork(FavoritesRepository favoritesRepository, FavoriteLabelContractBase data, int userId, long? favoriteLabelId, bool createDefault = false) : base(favoritesRepository)
        {
            m_favoritesRepository = favoritesRepository;
            m_data = data;
            m_userId = userId;
            m_favoriteLabelId = favoriteLabelId;
            m_createDefault = createDefault;
        }

        protected override long ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;
            var user = m_favoritesRepository.Load<User>(m_userId);

            FavoriteLabel favoriteLabel;
            if (m_favoriteLabelId == null)
            {
                favoriteLabel = new FavoriteLabel
                {
                    IsDefault = m_createDefault,
                    User = user,
                    // Other properties are set outside IF block
                };
            }
            else
            {
                favoriteLabel = m_favoritesRepository.FindById<FavoriteLabel>(m_favoriteLabelId.Value);

                OwnershipHelper.CheckItemOwnership(favoriteLabel.User.Id, m_userId);

                if (favoriteLabel.IsDefault)
                    throw new MainServiceException(MainServiceErrorCode.CannotModifyDefaultFavoriteLabel,"User can't modify default favorite label");
            }

            favoriteLabel.Name = m_data.Name;
            favoriteLabel.Color = m_data.Color;
            favoriteLabel.LastUseTime = now;

            m_favoritesRepository.Save(favoriteLabel);

            return favoriteLabel.Id;
        }
    }
}
