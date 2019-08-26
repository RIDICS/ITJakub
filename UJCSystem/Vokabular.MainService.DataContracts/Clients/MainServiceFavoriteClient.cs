using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Vokabular.MainService.DataContracts.Contracts.Favorite;
using Vokabular.RestClient;
using Vokabular.RestClient.Results;
using Vokabular.Shared;
using Vokabular.Shared.DataContracts.Types;
using Vokabular.Shared.DataContracts.Types.Favorite;
using Vokabular.Shared.Extensions;

namespace Vokabular.MainService.DataContracts.Clients
{
    public class MainServiceFavoriteClient
    {
        private static readonly ILogger m_logger = ApplicationLogging.CreateLogger<MainServiceRestClient>();
        private readonly MainServiceRestClient m_client;

        public MainServiceFavoriteClient(MainServiceRestClient client)
        {
            m_client = client;
        }

        public List<FavoriteLabelContract> GetFavoriteLabelList(int? count = null)
        {
            try
            {
                var result = m_client.Get<List<FavoriteLabelContract>>(UrlQueryBuilder.Create("favorite/label").AddParameter("count", count)
                    .ToQuery());
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public long CreateFavoriteLabel(FavoriteLabelContractBase data)
        {
            try
            {
                var result = m_client.Post<long>("favorite/label", data);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void UpdateFavoriteLabel(long favoriteLabelId, FavoriteLabelContractBase data)
        {
            try
            {
                m_client.Put<object>($"favorite/label/{favoriteLabelId}", data);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void DeleteFavoriteLabel(long favoriteLabelId)
        {
            try
            {
                m_client.Delete($"favorite/label/{favoriteLabelId}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public PagedResultList<FavoriteBaseInfoContract> GetFavoriteItems(int start, int count, long? filterByLabelId,
            FavoriteTypeEnumContract? filterByType, string filterByTitle, FavoriteSortEnumContract? sort)
        {
            try
            {
                var url = UrlQueryBuilder.Create("favorite")
                    .AddParameter("start", start)
                    .AddParameter("count", count)
                    .AddParameter("filterByLabelId", filterByLabelId)
                    .AddParameter("filterByType", filterByType)
                    .AddParameter("filterByTitle", filterByTitle)
                    .AddParameter("sort", sort)
                    .ToQuery();

                var result = m_client.GetPagedList<FavoriteBaseInfoContract>(url);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public PagedResultList<FavoriteQueryContract> GetFavoriteQueries(int start, int count, long? filterByLabelId,
            BookTypeEnumContract? bookType, QueryTypeEnumContract? queryType, string filterByTitle)
        {
            try
            {
                var url = UrlQueryBuilder.Create("favorite/query")
                    .AddParameter("start", start)
                    .AddParameter("count", count)
                    .AddParameter("filterByLabelId", filterByLabelId)
                    .AddParameter("bookType", bookType)
                    .AddParameter("queryType", queryType)
                    .AddParameter("filterByTitle", filterByTitle)
                    .ToQuery();

                var result = m_client.GetPagedList<FavoriteQueryContract>(url);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public List<FavoritePageContract> GetPageBookmarks(long projectId)
        {
            try
            {
                var result = m_client.Get<List<FavoritePageContract>>($"favorite/page?projectId={projectId}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public FavoriteFullInfoContract GetFavoriteItem(long favoriteId)
        {
            try
            {
                var result = m_client.Get<FavoriteFullInfoContract>($"favorite/{favoriteId}/detail");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void UpdateFavoriteItem(long favoriteId, UpdateFavoriteContract data)
        {
            try
            {
                m_client.Put<object>($"favorite/{favoriteId}", data);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void DeleteFavoriteItem(long favoriteId)
        {
            try
            {
                m_client.Delete($"favorite/{favoriteId}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public List<FavoriteBookGroupedContract> GetFavoriteLabeledBooks(IList<long> projectIds, BookTypeEnumContract? bookType)
        {
            try
            {
                var url = UrlQueryBuilder.Create("favorite/book/grouped")
                    .AddParameterList("projectIds", projectIds)
                    .AddParameter("bookType", bookType)
                    .ToQuery();

                var result = m_client.Get<List<FavoriteBookGroupedContract>>(url);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public List<FavoriteCategoryGroupedContract> GetFavoriteLabeledCategories()
        {
            try
            {
                var result = m_client.Get<List<FavoriteCategoryGroupedContract>>("favorite/category/grouped");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public List<FavoriteLabelWithBooksAndCategories> GetFavoriteLabelsWithBooksAndCategories(BookTypeEnumContract bookType)
        {
            try
            {
                var result = m_client.Get<List<FavoriteLabelWithBooksAndCategories>>(
                    $"favorite/label/with-books-and-categories?bookType={bookType.ToString()}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public long CreateFavoriteBook(CreateFavoriteProjectContract data)
        {
            try
            {
                var result = m_client.Post<long>("favorite/book", data);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public long CreateFavoriteCategory(CreateFavoriteCategoryContract data)
        {
            try
            {
                var result = m_client.Post<long>("favorite/category", data);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public long CreateFavoriteQuery(CreateFavoriteQueryContract data)
        {
            try
            {
                var result = m_client.Post<long>("favorite/query", data);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public long CreateFavoritePage(CreateFavoritePageContract data)
        {
            try
            {
                var result = m_client.Post<long>("favorite/page", data);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }
    }
}
