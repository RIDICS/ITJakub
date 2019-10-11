﻿using System.Collections.Generic;
using ITJakub.Web.Hub.Converters;
using ITJakub.Web.Hub.Core;
using ITJakub.Web.Hub.Models.Plugins.RegExSearch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Vokabular.RestClient.Errors;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Search.CriteriaItem;
using Vokabular.Shared.DataContracts.Search.Request;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.Web.Hub.Controllers.Plugins.Reader
{
    public class ReaderController : BaseController
    {
        public ReaderController(ControllerDataProvider controllerDataProvider) : base(controllerDataProvider)
        {
        }

        private JsonSerializerSettings GetJsonSerializerSettings()
        {
            return new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver()
            };
        }

        public ActionResult HasBookImage(long bookId, long? snapshotId)
        {
            var client = GetBookClient();
            return Json(new {hasBookImage = client.HasBookAnyImage(bookId)}, GetJsonSerializerSettings());
        }

        public ActionResult HasBookText(long bookId, long? snapshotId)
        {
            var client = GetBookClient();
            return Json(new {hasBookPage = client.HasBookAnyText(bookId)}, GetJsonSerializerSettings());
        }

        public ActionResult GetBookPage(long? snapshotId, long pageId)
        {
            var client = GetBookClient();
            var text = client.GetPageText(pageId, TextFormatEnumContract.Html);
            return Json(new {pageText = text}, GetJsonSerializerSettings());
        }

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public ActionResult GetBookImage(long? snapshotId, long pageId)
        {
            try
            {
                var client = GetBookClient();
                var imageData = client.GetPageImage(pageId);
                return new FileStreamResult(imageData.Stream, imageData.MimeType);
            }
            catch (HttpErrorCodeException e)
            {
                return StatusCode((int) e.StatusCode);
            }
        }

        public ActionResult GetTermsOnPage(string snapshotId, long pageId)
        {
            var client = GetBookClient();
            var terms = client.GetPageTermList(pageId);
            return Json(new {terms});
        }

        private List<SearchCriteriaContract> CreateQueryCriteriaContract(CriteriaKey criteriaKey, string query)
        {
            return new List<SearchCriteriaContract>
            {
                new WordListCriteriaContract
                {
                    Key = criteriaKey,
                    Disjunctions = new List<WordCriteriaContract>
                    {
                        new WordCriteriaContract
                        {
                            Contains = new List<string> {query}
                        }
                    }
                }
            };
        }

        public ActionResult GetBookSearchPageByXmlId(string query, bool isQueryJson, long? snapshotId, long pageId)
        {
            List<SearchCriteriaContract> listSearchCriteriaContracts;
            if (isQueryJson)
            {
                var deserialized = JsonConvert.DeserializeObject<IList<ConditionCriteriaDescriptionBase>>(query,
                    new ConditionCriteriaDescriptionConverter());
                listSearchCriteriaContracts = Mapper.Map<List<SearchCriteriaContract>>(deserialized);
            }
            else
            {
                listSearchCriteriaContracts = CreateQueryCriteriaContract(CriteriaKey.Fulltext, query);
            }

            var request = new SearchPageRequestContract
            {
                ConditionConjunction = listSearchCriteriaContracts
            };
            var client = GetBookClient();
            var text = client.GetPageTextFromSearch(pageId, TextFormatEnumContract.Html, request);
            return Json(new {pageText = text}, GetJsonSerializerSettings());
        }

        public ActionResult GetBookContent(long bookId)
        {
            var client = GetBookClient();
            var contentItems = client.GetBookChapterList(bookId);
            return Json(new {content = contentItems});
        }

        public ActionResult GetEditionNote(long projectId, TextFormatEnumContract format)
        {
            var client = GetBookClient();
            var editionNote = client.GetEditionNoteText(projectId, TextFormatEnumContract.Html);
            return Json(new { editionNote }, GetJsonSerializerSettings());
        }

        public ActionResult GetProjectDetail(long projectId)
        {
            var client = GetBookClient();
            var projectDetail = client.GetBookDetail(projectId);
            return Json(new {detail = projectDetail}, GetJsonSerializerSettings());
        }

        public ActionResult GetAudioBook(long projectId)
        {
            var client = GetBookClient();
            var audioBook = client.GetAudioBookDetail(projectId);

            return Json(new {audioBook}, GetJsonSerializerSettings());
        }

        public ActionResult GetAudioBookTrack(long projectId, int trackId)
        {
            var client = GetBookClient();
            var audioBookTrack = client.GetAudioBookDetail(projectId).Tracks[trackId];
            return Json(new { track = audioBookTrack }, GetJsonSerializerSettings());
        }
    }
}