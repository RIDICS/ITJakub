using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Search;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.Shared.DataContracts.Search.Request;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class HeadwordController : BaseController
    {
        private readonly BookManager m_bookManager;
        private readonly BookSearchManager m_bookSearchManager;

        public HeadwordController(BookManager bookManager, BookSearchManager bookSearchManager)
        {
            m_bookManager = bookManager;
            m_bookSearchManager = bookSearchManager;
        }

        /// <summary>
        /// Search headwords
        /// </summary>
        /// <remarks>
        /// Search headwords. Supported search criteria (key property - data type):
        /// - Author - WordListCriteriaContract
        /// - Title - WordListCriteriaContract
        /// - Editor - WordListCriteriaContract
        /// - Fulltext - WordListCriteriaContract
        /// - Heading - WordListCriteriaContract
        /// - Sentence - WordListCriteriaContract
        /// - Headword - WordListCriteriaContract
        /// - HeadwordDescription - WordListCriteriaContract
        /// - Term - WordListCriteriaContract
        /// - Dating - DatingListCriteriaContract
        /// - TokenDistance - TokenDistanceListCriteriaContract
        /// - HeadwordDescriptionTokenDistance - TokenDistanceListCriteriaContract
        /// - SelectedCategory - SelectedCategoryCriteriaContract
        /// </remarks>
        /// <param name="request">
        /// Request contains list of search criteria with different data types described in method description
        /// </param>
        /// <param name="projectType">Target project database for searching</param>
        /// <returns></returns>
        [HttpPost("search")]
        public ActionResult<List<HeadwordContract>> SearchHeadword([FromBody] HeadwordSearchRequestContract request, [FromQuery] ProjectTypeContract? projectType)
        {
            if (projectType == null)
            {
                return Error($"Required parameter {nameof(projectType)} is not specified");
            }

            var result = m_bookSearchManager.SearchHeadwordByCriteria(request);
            return result;
        }

        /// <summary>
        /// Search audio books, return count
        /// </summary>
        /// <param name="request"></param>
        /// <param name="projectType"></param>
        /// <returns></returns>
        [HttpPost("search-count")]
        public ActionResult<long> SearchHeadwordResultCount([FromBody] HeadwordSearchRequestContract request, [FromQuery] ProjectTypeContract? projectType)
        {
            if (projectType == null)
            {
                return Error($"Required parameter {nameof(projectType)} is not specified");
            }

            var result = m_bookSearchManager.SearchHeadwordByCriteriaCount(request);
            return result;
        }

        /// <summary>
        /// Search for location of specified headword
        /// </summary>
        /// <remarks>
        /// Search for headword location in headword listing (in list with all headwords from selected dictionaries sorted by name)
        /// </remarks>
        /// <param name="request"></param>
        /// <param name="projectType"></param>
        /// <returns></returns>
        [HttpPost("search-row-number")]
        public ActionResult<long> SearchHeadwordRowNumber([FromBody] HeadwordRowNumberSearchRequestContract request, [FromQuery] ProjectTypeContract? projectType)
        {
            if (projectType == null)
            {
                return Error($"Required parameter {nameof(projectType)} is not specified");
            }

            var result = m_bookManager.SearchHeadwordRowNumber(request);
            return result;
        }

        [HttpGet("autocomplete")]
        public ActionResult<List<string>> GetAutocomplete([FromQuery] string query, [FromQuery] ProjectTypeContract? projectType, BookTypeEnumContract? bookType = null, IList<int> selectedCategoryIds = null, IList<long> selectedProjectIds = null)
        {
            if (projectType == null)
            {
                return Error($"Required parameter {nameof(projectType)} is not specified");
            }

            return m_bookManager.GetHeadwordAutocomplete(query, bookType, selectedCategoryIds, selectedProjectIds);
        }
    }
}