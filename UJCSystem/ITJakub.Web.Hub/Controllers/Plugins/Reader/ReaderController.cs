using System.Collections.Generic;
using System.Net.Http;
using AutoMapper;
using ITJakub.Web.Hub.Converters;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Models.Plugins.RegExSearch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Vokabular.MainService.DataContracts.Contracts.Search;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.RestClient.Errors;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Search.CriteriaItem;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.Web.Hub.Controllers.Plugins.Reader
{
    public class ReaderController : BaseController
    {
        public ReaderController(CommunicationProvider communicationProvider) : base(communicationProvider)
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
            using (var client = GetRestClient())
            {
                return Json(new { HasBookImage = client.HasBookAnyImage(bookId) });
            }
        }

        public ActionResult HasBookText(long bookId, long? snapshotId)
        {
            using (var client = GetRestClient())
            {
                return Json(new {HasBookPage = client.HasBookAnyText(bookId)}, GetJsonSerializerSettings());
            }
        }

        public ActionResult GetBookPage(long? snapshotId, long pageId)
        {
            using (var client = GetRestClient())
            {
                var text = client.GetPageText(pageId, TextFormatEnumContract.Html);
                return Json(new { pageText = text }, GetJsonSerializerSettings());
            }
        }

        public ActionResult GetBookImage(long? snapshotId, long pageId)
        {
            using (var client = GetRestClient())
            {
                try
                {
                    var imageData = client.GetPageImage(pageId);
                    return new FileStreamResult(imageData.Stream, imageData.MimeType);
                }
                catch (HttpErrorCodeException e)
                {
                    return StatusCode((int) e.StatusCode);
                }
            }
        }

        public ActionResult GetTermsOnPage(string snapshotId, long pageId)
        {
            using (var client = GetRestClient())
            {
                var terms = client.GetPageTermList(pageId);
                return Json(new { terms });
            }
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

            using (var client = GetRestClient())
            {
                var request = new SearchPageRequestContract
                {
                    ConditionConjunction = listSearchCriteriaContracts
                };
                var text = client.GetPageTextFromSearch(pageId, TextFormatEnumContract.Html, request);
                return Json(new {pageText = text}, GetJsonSerializerSettings());
            }
        }

        public ActionResult GetBookContent(long bookId)
        {
            using (var client = GetRestClient())
            {
                var contentItems = client.GetBookChapterList(bookId);
                return Json(new { content = contentItems });
            }
        }

        public ActionResult GetEditionNote(long projectId, TextFormatEnumContract format)
        {
            using (var client = GetRestClient())
            {
                var editionNote = client.GetEditionNote(projectId, TextFormatEnumContract.Html);
                return Json(new { editionNote = editionNote }, GetJsonSerializerSettings());
            }
        }

        public ActionResult GetProjectDetail(long projectId)
        {
            using (var client = GetRestClient())
            {
                var projectDetail = client.GetBookDetail(projectId);
                return Json(new {detail = projectDetail}, GetJsonSerializerSettings());
            }
        }

        public ActionResult GetAudioBook(long projectId)
        {
            using (var client = GetRestClient())
            {
                var audioBook = client.GetAudioBookDetail(projectId);
                var bookRecordings = audioBook.FullBookRecordings;

                return Json(new { audioBook = audioBook, fullBookRec = bookRecordings }, GetJsonSerializerSettings());
            }
        }

        public ActionResult GetAudioBookTrack(long projectId, int trackId)
        {
            using (var client = GetRestClient())
            {
                var audioBookTrack = client.GetAudioBookDetail(projectId).Tracks[trackId];
                return Json(new { track = audioBookTrack }, GetJsonSerializerSettings());
            }
        }
    }
}