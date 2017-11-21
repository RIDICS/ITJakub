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
        List<FavoriteBaseInfoContract> GetFavoriteItems([FromQuery] int? start,
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
        IActionResult GetFavoriteQueries([FromQuery] int? start,
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
        FavoriteFullInfoContract GetFavoriteItem(long favoriteId)
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
        List<FavoriteBookGroupedContract> GetFavoriteLabeledBooks([FromQuery] IList<long> projectIds)
        {
            // TODO Consider getting books and categories in one request (ProjectId WHERE IN database limit problem)
            throw new NotImplementedException();
        }

        [HttpGet("category/grouped")]
        List<FavoriteCategoryGroupedContract> GetFavoriteLabeledCategories([FromQuery] IList<int> categoryIds)
        {
            throw new NotImplementedException();
        }

        [HttpGet("label/fetch-books-and-categories")]
        public void GetFavoriteLabelsWithBooksAndCategories([FromQuery] BookTypeEnumContract? bookType)
        {
            // TODO is this method required? is correct?
            // TODO specify response type
            throw new NotImplementedException();
        }

        //TODO create methods for Favorite items
    }
}