using System.Collections.Generic;
using AutoMapper;
using ITJakub.Shared.Contracts;
using ITJakub.Web.Hub.Converters;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Models.Plugins.RegExSearch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
            //using (var client = GetMainServiceClient())
            //{
            //    var text = client.GetBookPageByXmlId(bookId, pageXmlId, OutputFormatEnumContract.Html,
            //        BookTypeEnumContract.Edition);
            //    return Json(new {pageText = text}, GetJsonSerializerSettings());
            //}
            return NotFound();
        }

        public ActionResult GetBookImage(long? snapshotId, long pageId)
        {
            //using (var client = GetMainServiceClient())
            //{
            //    var imageDataStream = client.GetBookPageImage(bookId, position);
            //    return new FileStreamResult(imageDataStream, "image/jpeg"); //TODO resolve content type properly
            //}
            return NotFound();
        }

        public ActionResult GetTermsOnPage(string bookId, string pageXmlId)
        {
            using (var client = GetMainServiceClient())
            {
                var terms = client.GetTermsOnPage(bookId, pageXmlId);
                return Json(new {terms}, GetJsonSerializerSettings());
            }
        }


        public ActionResult GetBookSearchPageByXmlId(string query, bool isQueryJson, string bookId, string pageXmlId)
        {
            IList<SearchCriteriaContract> listSearchCriteriaContracts;
            if (isQueryJson)
            {
                var deserialized = JsonConvert.DeserializeObject<IList<ConditionCriteriaDescriptionBase>>(query,
                    new ConditionCriteriaDescriptionConverter());
                listSearchCriteriaContracts = Mapper.Map<IList<SearchCriteriaContract>>(deserialized);
            }
            else
            {
                listSearchCriteriaContracts = new List<SearchCriteriaContract>
                {
                    new WordListCriteriaContract
                    {
                        Key = CriteriaKey.Fulltext,
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
            using (var client = GetMainServiceClient())
            {
                var text = client.GetEditionPageFromSearch(listSearchCriteriaContracts, bookId, pageXmlId,
                    OutputFormatEnumContract.Html);
                return Json(new {pageText = text}, GetJsonSerializerSettings());
            }
        }

        public ActionResult GetBookPageList(string bookId)
        {
            using (var client = GetMainServiceClient())
            {
                var pages = client.GetBookPageList(bookId);
                return Json(new {pageList = pages}, GetJsonSerializerSettings());
            }
        }

        public ActionResult GetBookContent(string bookId)
        {
            using (var client = GetMainServiceClient())
            {
                var contentItems = client.GetBookContent(bookId);
                return Json(new {content = contentItems}, GetJsonSerializerSettings());
            }
        }
    }
}