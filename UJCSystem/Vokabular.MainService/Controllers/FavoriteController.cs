using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts.Favorite;
using Vokabular.MainService.Utils.Documentation;
using Vokabular.RestClient.Headers;
using Vokabular.Shared.DataContracts.Types;
using Vokabular.Shared.DataContracts.Types.Favorite;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class FavoriteController : BaseController
    {
        private readonly FavoriteManager m_favoriteManager;
        private const int MaxProjectIdsCount = 200;

        public FavoriteController(FavoriteManager favoriteManager)
        {
            m_favoriteManager = favoriteManager;
        }

        [HttpGet("label")]
        public List<FavoriteLabelContract> GetFavoriteLabelList([FromQuery] int? count)
        {
            var result = m_favoriteManager.GetFavoriteLabels(count);
            return result;
        }

        [HttpPost("label")]
        public IActionResult CreateFavoriteLabel([FromBody] FavoriteLabelContractBase data)
        {
            var resultId = m_favoriteManager.CreateFavoriteLabel(data);
            return Ok(resultId);
        }

        [HttpPut("label/{favoriteLabelId}")]
        public IActionResult UpdateFavoriteLabel(long favoriteLabelId, [FromBody] FavoriteLabelContractBase data)
        {
            m_favoriteManager.UpdateFavoriteLabel(favoriteLabelId, data);
            return Ok();
        }

        [HttpDelete("label/{favoriteLabelId}")]
        public IActionResult DeleteFavoriteLabel(long favoriteLabelId)
        {
            m_favoriteManager.DeleteFavoriteLabel(favoriteLabelId);
            return Ok();
        }

        [HttpGet("")]
        [ProducesResponseTypeHeader(StatusCodes.Status200OK, CustomHttpHeaders.TotalCount, ResponseDataType.Integer, "Total count")]
        public List<FavoriteBaseInfoContract> GetFavoriteItems([FromQuery] int? start,
            [FromQuery] int? count,
            [FromQuery] long? filterByLabelId,
            [FromQuery] FavoriteTypeEnumContract? filterByType,
            [FromQuery] string filterByTitle,
            [FromQuery] FavoriteSortEnumContract? sort)
        {
            var sortValue = sort ?? FavoriteSortEnumContract.TitleAsc;
            var result = m_favoriteManager.GetFavoriteItems(start, count, filterByLabelId, filterByType, filterByTitle, sortValue);

            SetTotalCountHeader(result.TotalCount);

            return result.List;
        }

        [HttpGet("query")]
        [ProducesResponseType(typeof(List<FavoriteQueryContract>), StatusCodes.Status200OK)]
        [ProducesResponseTypeHeader(StatusCodes.Status200OK, CustomHttpHeaders.TotalCount, ResponseDataType.Integer, "Total count")]
        public IActionResult GetFavoriteQueries([FromQuery] int? start,
            [FromQuery] int? count,
            [FromQuery] long? filterByLabelId,
            [FromQuery] BookTypeEnumContract? bookType,
            [FromQuery] QueryTypeEnumContract? queryType,
            [FromQuery] string filterByTitle)
        {
            if (bookType == null || queryType == null)
            {
                return BadRequest("Missing required parameters BookType and QueryType");
            }

            var result = m_favoriteManager.GetFavoriteQueries(start, count, filterByLabelId, bookType.Value, queryType.Value, filterByTitle);

            SetTotalCountHeader(result.TotalCount);

            return Ok(result.List);
        }

        [HttpGet("page")]
        [ProducesResponseType(typeof(List<FavoritePageContract>), StatusCodes.Status200OK)]
        public IActionResult GetPageBookmarks([FromQuery] long? projectId)
        {
            if (projectId == null)
            {
                return BadRequest("Missing required parameter ProjectId");
            }

            var result = m_favoriteManager.GetPageBookmarks(projectId.Value);
            return Ok(result);
        }

        [HttpGet("{favoriteId}/detail")]
        public FavoriteFullInfoContract GetFavoriteItem(long favoriteId)
        {
            var result = m_favoriteManager.GetFavoriteItem(favoriteId);
            return result;
        }

        [HttpPut("{favoriteId}")]
        public void UpdateFavorite(long favoriteId, [FromBody] UpdateFavoriteContract data)
        {
            m_favoriteManager.UpdateFavoriteItem(favoriteId, data);
        }

        [HttpDelete("{favoriteId}")]
        public void DeleteFavorite(long favoriteId)
        {
            m_favoriteManager.DeleteFavoriteItem(favoriteId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectIds">List of project ID (disjunction with BookType)</param>
        /// <param name="bookType">BookType (disjunction with ProjectIds)</param>
        /// <returns></returns>
        [HttpGet("book/grouped")]
        [ProducesResponseType(typeof(List<FavoriteBookGroupedContract>), StatusCodes.Status200OK)]
        public IActionResult GetFavoriteLabeledBooks([FromQuery] IList<long> projectIds, [FromQuery] BookTypeEnumContract? bookType)
        {
            if (bookType == null && projectIds.Count == 0)
            {
                return BadRequest("Missing required parameter BookType or ProjectIds");
            }
            if (projectIds.Count > MaxProjectIdsCount)
            {
                return BadRequest($"Max ProjectIds count is limited to {MaxProjectIdsCount}");
            }

            var result = m_favoriteManager.GetFavoriteLabeledBooks(projectIds, bookType);
            return Ok(result);
        }

        [HttpGet("category/grouped")]
        public List<FavoriteCategoryGroupedContract> GetFavoriteLabeledCategories(/*[FromQuery] IList<int> categoryIds*/)
        {
            var result = m_favoriteManager.GetFavoriteLabeledCategories();
            return result;
        }

        [HttpGet("label/with-books-and-categories")]
        [ProducesResponseType(typeof(List<FavoriteLabelWithBooksAndCategories>), StatusCodes.Status200OK)]
        public IActionResult GetFavoriteLabelsWithBooksAndCategories([FromQuery] BookTypeEnumContract? bookType)
        {
            if (bookType == null)
            {
                return BadRequest("Missing required parameter BookType");
            }

            var result = m_favoriteManager.GetFavoriteLabelsWithBooksAndCategories(bookType.Value);
            return Ok(result);
        }
        
        [HttpPost("book")]
        public long CreateFavoriteBook([FromBody] CreateFavoriteProjectContract data)
        {
            var resultId = m_favoriteManager.CreateFavoriteProject(data);
            return resultId;
        }

        [HttpPost("category")]
        public long CreateFavoriteCategory([FromBody] CreateFavoriteCategoryContract data)
        {
            var resultId = m_favoriteManager.CreateFavoriteCategory(data);
            return resultId;
        }

        [HttpPost("query")]
        public long CreateFavoriteQuery([FromBody] CreateFavoriteQueryContract data)
        {
            var resultId = m_favoriteManager.CreateFavoriteQuery(data);
            return resultId;
        }

        [HttpPost("page")]
        public long CreateFavoritePage([FromBody] CreateFavoritePageContract data)
        {
            var resultId = m_favoriteManager.CreateFavoritePage(data);
            return resultId;
        }
    }
}