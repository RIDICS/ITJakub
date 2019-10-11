using System;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts.Contracts.Favorite;

namespace Vokabular.MainService.Core.Works.Favorite
{
    public class CreateFavoriteQueryWork : CreateFavoriteItemWorkBase
    {
        private readonly FavoritesRepository m_favoritesRepository;
        private readonly CatalogValueRepository m_catalogValueRepository;
        private readonly CreateFavoriteQueryContract m_data;
        private readonly int m_userId;
        private readonly IMapper m_mapper;

        public CreateFavoriteQueryWork(FavoritesRepository favoritesRepository, CatalogValueRepository catalogValueRepository, CreateFavoriteQueryContract data, int userId, IMapper mapper) : base(favoritesRepository)
        {
            m_favoritesRepository = favoritesRepository;
            m_catalogValueRepository = catalogValueRepository;
            m_data = data;
            m_userId = userId;
            m_mapper = mapper;
        }

        protected override long ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;
            var user = m_favoritesRepository.Load<User>(m_userId);

            var bookTypeEnum = m_mapper.Map<BookTypeEnum>(m_data.BookType);
            var queryTypeEnum = m_mapper.Map<QueryTypeEnum>(m_data.QueryType);

            var bookTypeEntity = m_catalogValueRepository.GetBookType(bookTypeEnum);
            var label = GetFavoriteLabelAndCheckAuthorization(m_data.FavoriteLabelId, user.Id);

            label.LastUseTime = now;

            var favoriteItem = new FavoriteQuery
            {
                BookType = bookTypeEntity,
                Query = m_data.Query,
                QueryType = queryTypeEnum,
                CreateTime = now,
                FavoriteLabel = label,
                Title = m_data.Title,
            };

            var resultId = (long)m_favoritesRepository.Create(favoriteItem);
            return resultId;
        }
    }
}