using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Search;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class HeadwordController : Controller
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
        /// <returns></returns>
        [HttpPost("search")]
        public List<HeadwordContract> SearchHeadword([FromBody] HeadwordSearchRequestContract request)
        {
            var result = m_bookSearchManager.SearchHeadwordByCriteria(request);
            return result;
        }

        /// <summary>
        /// Search audio books, return count
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("search-count")]
        public long SearchHeadwordResultCount([FromBody] HeadwordSearchRequestContract request)
        {
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
        /// <returns></returns>
        [HttpPost("search-row-number")]
        public long SearchHeadwordRowNumber([FromBody] HeadwordRowNumberSearchRequestContract request)
        {
            var result = m_bookManager.SearchHeadwordRowNumber(request);
            return result;
        }

        [HttpGet("autocomplete")]
        public List<string> GetAutocomplete([FromQuery] string query, BookTypeEnumContract? bookType = null, IList<int> selectedCategoryIds = null, IList<long> selectedProjectIds = null)
        {
            return m_bookManager.GetHeadwordAutocomplete(query, bookType, selectedCategoryIds, selectedProjectIds);
        }
    }
}