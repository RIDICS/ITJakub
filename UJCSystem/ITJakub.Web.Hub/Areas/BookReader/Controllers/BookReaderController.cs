using System.Collections.Generic;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Converters;
using ITJakub.Web.Hub.Core;
using ITJakub.Web.Hub.Models;
using ITJakub.Web.Hub.Models.Plugins.RegExSearch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Vokabular.Shared.AspNetCore.Extensions;
using Vokabular.Shared.Const;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Search.Request;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.Web.Hub.Areas.BookReader.Controllers
{
    [Area("BookReader")]
    public class BookReaderController : AreaController
    {
        public BookReaderController(ControllerDataProvider controllerDataProvider) : base(controllerDataProvider)
        {
        }

        protected override BookTypeEnumContract AreaBookType => BookTypeEnumContract.Edition;
        
        #region Views and Feedback
                
        public ActionResult Listing(long? bookId, string searchText, string pageId, string searchType = "Fulltext")
        {
            if (bookId == null)
            {
                return BadRequest();
            }

            var projectClient = GetProjectClient();
            var snapshotInfo = projectClient.GetLatestPublishedSnapshot(bookId.Value);

            if (snapshotInfo == null)
            {
                return NotFound();
            }

            switch (snapshotInfo.DefaultBookType)
            {
                case BookTypeEnumContract.Dictionary:
                    return RedirectToAction("Listing", "Dictionaries", new
                    {
                        Area = "Dictionaries",
                        books = $"[{bookId}]"
                    });
                case BookTypeEnumContract.BibliographicalItem:
                    // No direct access to book (content doesn't exist), but at least display the bibliography list
                    return RedirectToAction("Search", "Bibliographies", new
                    {
                        Area = "Bibliographies",
                    });
                case BookTypeEnumContract.CardFile:
                    return BadRequest();
            }

            var client = GetBookClient();
            var book = client.GetBookInfo(bookId.Value);
            var pages = client.GetBookPageList(bookId.Value);
            return
                View(new BookListingModel
                {
                    BookId = book.Id,
                    SnapshotId = null, 
                    BookTitle = book.Title,
                    BookPages = pages,
                    SearchText = searchText,
                    SearchType = searchType,
                    InitPageId = pageId, 
                    CanPrintEdition = User.HasPermission(VokabularPermissionNames.EditionPrintText),
                    JsonSerializerSettingsForBiblModule = GetJsonSerializerSettingsForBiblModule()
                });
        }
        
        #endregion

        #region Search in book

        public ActionResult AdvancedSearchInBookPaged(string json, int start, int count, long projectId, long? snapshotId)
        {
            var deserialized = JsonConvert.DeserializeObject<IList<ConditionCriteriaDescriptionBase>>(json, new ConditionCriteriaDescriptionConverter());
            var listSearchCriteriaContracts = Mapper.Map<IList<SearchCriteriaContract>>(deserialized);

            var client = GetBookClient();
            var result = client.SearchHitsWithPageContext(projectId, new SearchHitsRequestContract
            {
                ConditionConjunction = listSearchCriteriaContracts,
                ContextLength = 45,
                Count = count,
                Start = start,
            });

            return Json(new { results = result }, GetJsonSerializerSettingsForBiblModule());
        }

        public ActionResult AdvancedSearchInBookCount(string json, long projectId, long? snapshotId)
        {
            var deserialized = JsonConvert.DeserializeObject<IList<ConditionCriteriaDescriptionBase>>(json, new ConditionCriteriaDescriptionConverter());
            var listSearchCriteriaContracts = Mapper.Map<IList<SearchCriteriaContract>>(deserialized);

            var client = GetBookClient();
            var resultCount = client.SearchHitsResultCount(projectId, new SearchHitsRequestContract
            {
                ConditionConjunction = listSearchCriteriaContracts
            });

            return Json(new { count = resultCount });
        }

        public ActionResult AdvancedSearchInBookPagesWithMatchHit(string json, long projectId, long? snapshotId)
        {
            var deserialized = JsonConvert.DeserializeObject<IList<ConditionCriteriaDescriptionBase>>(json, new ConditionCriteriaDescriptionConverter());
            var listSearchCriteriaContracts = Mapper.Map<List<SearchCriteriaContract>>(deserialized);

            var client = GetBookClient();
            var request = new SearchPageRequestContract
            {
                ConditionConjunction = listSearchCriteriaContracts
            };
            var result = client.SearchPage(projectId, request);

            return Json(new { pages = result });
        }

        public ActionResult TextSearchInBookPaged(string text, int start, int count, long projectId, long? snapshotId)
        {
            var listSearchCriteriaContracts = CreateTextCriteriaList(CriteriaKey.Fulltext, text);

            var client = GetBookClient();
            var result = client.SearchHitsWithPageContext(projectId, new SearchHitsRequestContract
            {
                ConditionConjunction = listSearchCriteriaContracts,
                ContextLength = 45,
                Count = count,
                Start = start,
            });

            return Json(new { results = result }, GetJsonSerializerSettingsForBiblModule());
        }

        public ActionResult TextSearchInBookCount(string text, long projectId, long? snapshotId)
        {
            var listSearchCriteriaContracts = CreateTextCriteriaList(CriteriaKey.Fulltext, text);

            var client = GetBookClient();
            var resultCount = client.SearchHitsResultCount(projectId, new SearchHitsRequestContract
            {
                ConditionConjunction = listSearchCriteriaContracts
            });

            return Json(new { count = resultCount });
        }

        public ActionResult TextSearchInBookPagesWithMatchHit(string text, long projectId, long? snapshotId)
        {
            var listSearchCriteriaContracts = CreateTextCriteriaList(CriteriaKey.Fulltext, text);

            var client = GetBookClient();
            var request = new SearchPageRequestContract
            {
                ConditionConjunction = listSearchCriteriaContracts
            };
            var result = client.SearchPage(projectId, request);

            return Json(new { pages = result });
        }

        #endregion
    }
}