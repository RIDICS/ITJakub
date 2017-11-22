using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.DataContracts.Contracts.Favorite;
using Vokabular.MainService.DataContracts.Contracts.Favorite.Type;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class FavoriteController : BaseController
    {
        private const int MaxProjectIdsCount = 200;

        public FavoriteController()
        {
        }

        [HttpGet("label")]
        public List<FavoriteLabelContract> GetFavoriteLabelList([FromQuery] int? count)
        {
            throw new NotImplementedException();
        }

        [HttpPost("label")]
        public IActionResult CreateFavoriteLabel([FromBody] FavoriteLabelContractBase data)
        {
            throw new NotImplementedException();
        }

        [HttpPut("label")]
        public IActionResult UpdateFavoriteLabel(long favoriteLabelId, [FromBody] FavoriteLabelContractBase data)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("label")]
        public IActionResult DeleteFavoriteLabel(long favoriteLabelId)
        {
            throw new NotImplementedException();
        }

        [HttpGet("")]
        public List<FavoriteBaseInfoContract> GetFavoriteItems([FromQuery] int? start,
            [FromQuery] int? count,
            [FromQuery] long? labelId,
            [FromQuery] FavoriteTypeEnumContract? filterByType,
            [FromQuery] string filterByTitle,
            [FromQuery] FavoriteSortEnumContract? sort)
        {
            SetTotalCountHeader(0);

            throw new NotImplementedException();
        }

        [HttpGet("query")]
        [ProducesResponseType(typeof(List<FavoriteQueryContract>), StatusCodes.Status200OK)]
        public IActionResult GetFavoriteQueries([FromQuery] int? start,
            [FromQuery] int? count,
            [FromQuery] long? labelId,
            [FromQuery] BookTypeEnumContract? bookType,
            [FromQuery] QueryTypeEnumContract? queryType,
            [FromQuery] string filterByTitle)
        {
            if (bookType == null || queryType == null)
            {
                return BadRequest("Missing required parameters BookType and QueryType");
            }

            SetTotalCountHeader(0);

            throw new NotImplementedException();
        }

        [HttpGet("page")]
        [ProducesResponseType(typeof(List<FavoritePageContract>), StatusCodes.Status200OK)]
        public IActionResult GetPageBookmarks([FromQuery] long? projectId)
        {
            if (projectId == null)
            {
                return BadRequest("Missing required parameter ProjectId");
            }

            //TODO possibly change URL to book/{projectId}/favorite-page

            throw new NotImplementedException();
        }

        [HttpGet("{favoriteId}/detail")]
        public FavoriteFullInfoContract GetFavoriteItem(long favoriteId)
        {
            throw new NotImplementedException();
        }

        [HttpPut("{favoriteId}")]
        public void UpdateFavorite(long favoriteId, [FromBody] UpdateFavoriteContract data)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("{favoriteId}")]
        public void DeleteFavorite(long favoriteId)
        {
            throw new NotImplementedException();
        }

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
            
            // TODO update UI for using BookType parameter

            throw new NotImplementedException();
        }

        [HttpGet("category/grouped")]
        public List<FavoriteCategoryGroupedContract> GetFavoriteLabeledCategories(/*[FromQuery] IList<int> categoryIds*/)
        {
            throw new NotImplementedException();
        }

        [HttpGet("label/with-books-and-categories")]
        [ProducesResponseType(typeof(List<FavoriteLabelWithBooksAndCategories>), StatusCodes.Status200OK)]
        public IActionResult GetFavoriteLabelsWithBooksAndCategories([FromQuery] BookTypeEnumContract? bookType)
        {
            if (bookType == null)
            {
                return BadRequest("Missing required parameter BookType");
            }

            throw new NotImplementedException();
        }

        //TODO create methods for Favorite items

        [HttpPost("book")]
        public long CreateFavoriteBook([FromBody] CreateFavoriteProjectContract data)
        {
            throw new NotImplementedException();
        }

        [HttpPost("category")]
        public long CreateFavoriteCategory([FromBody] CreateFavoriteCategoryContract data)
        {
            throw new NotImplementedException();
        }

        [HttpPost("query")]
        public long CreateFavoriteQuery([FromBody] CreateFavoriteQueryContract data)
        {
            throw new NotImplementedException();
        }

        [HttpPost("page")]
        public long CreatePageBookmark([FromBody] CreateFavoritePageContract data)
        {
            throw new NotImplementedException();
        }
    }
}