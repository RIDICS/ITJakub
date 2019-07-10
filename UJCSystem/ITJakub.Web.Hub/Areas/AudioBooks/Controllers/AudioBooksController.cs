using System.Collections.Generic;
using AutoMapper;
using ITJakub.Web.Hub.Authorization;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Converters;
using ITJakub.Web.Hub.Core;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Core.Managers;
using ITJakub.Web.Hub.Models;
using ITJakub.Web.Hub.Models.Plugins.RegExSearch;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.Shared;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Search.Request;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.Web.Hub.Areas.AudioBooks.Controllers
{
    [LimitedAccess(PortalType.ResearchPortal)]
    [Area("AudioBooks")]
    public class AudioBooksController : AreaController
    {
        private readonly StaticTextManager m_staticTextManager;
        private readonly FeedbacksManager m_feedbacksManager;

        public AudioBooksController(StaticTextManager staticTextManager, FeedbacksManager feedbacksManager, CommunicationProvider communicationProvider) : base(communicationProvider)
        {
            m_staticTextManager = staticTextManager;
            m_feedbacksManager = feedbacksManager;
        }

        protected override BookTypeEnumContract AreaBookType => BookTypeEnumContract.AudioBook;

        private FeedbackFormIdentification GetFeedbackFormIdentification()
        {
            return new FeedbackFormIdentification {Area = "AudioBooks", Controller = "AudioBooks"};
        }

        public ActionResult Index()
        {
            return View("List");
        }

        public ActionResult Information()
        {
            var pageStaticText = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextAudioBooksInfo, "audio");
            return View(pageStaticText);
        }

        public ActionResult Feedback()
        {
            var viewModel = m_feedbacksManager.GetBasicViewModel(GetFeedbackFormIdentification(), StaticTexts.TextHomeFeedback, IsUserLoggedIn(), "home", User);
            return View(viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Feedback(FeedbackViewModel model)
        {
            if (!ModelState.IsValid)
            {
                m_feedbacksManager.FillViewModel(model, StaticTexts.TextHomeFeedback, "home", GetFeedbackFormIdentification());
                return View(model);
            }

            m_feedbacksManager.CreateFeedback(model, FeedbackCategoryEnumContract.AudioBooks, IsUserLoggedIn());
            return View("Feedback/FeedbackSuccess");
        }

        public ActionResult List()
        {
            return View();
        }

        
        public ActionResult GetAudioWithCategories()
        {
            var result = GetBooksAndCategories();
            return Json(result);
        }

        public ActionResult AdvancedSearchResultsCount(string json, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var count = SearchByCriteriaJsonCount(json, selectedBookIds, selectedCategoryIds);
            return Json(new { count });
        }

        public ActionResult AdvancedSearchPaged(string json, int start, int count, short sortingEnum, bool sortAsc, IList<long> selectedBookIds,
            IList<int> selectedCategoryIds)
        {
            var deserialized = JsonConvert.DeserializeObject<IList<ConditionCriteriaDescriptionBase>>(json, new ConditionCriteriaDescriptionConverter());
            var listSearchCriteriaContracts = Mapper.Map<List<SearchCriteriaContract>>(deserialized);

            AddCategoryCriteria(listSearchCriteriaContracts, selectedBookIds, selectedCategoryIds);

            using (var client = GetRestClient())
            {
                var request = new SearchRequestContract
                {
                    ConditionConjunction = listSearchCriteriaContracts,
                    Start = start,
                    Count = count,
                    Sort = (SortTypeEnumContract)sortingEnum,
                    SortDirection = sortAsc ? SortDirectionEnumContract.Asc : SortDirectionEnumContract.Desc,
                };
                var results = client.SearchAudioBook(request);
                return Json(new {books = results}, GetJsonSerializerSettingsForBiblModule());
            }
        }

        public ActionResult TextSearchCount(string text, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var count = SearchByCriteriaTextCount(CriteriaKey.Title, text, selectedBookIds, selectedCategoryIds);
            return Json(new { count });
        }
        
        public ActionResult TextSearchPaged(string text, int start, int count, short sortingEnum, bool sortAsc, IList<long> selectedBookIds,
            IList<int> selectedCategoryIds)
        {
            var listSearchCriteriaContracts = CreateTextCriteriaList(CriteriaKey.Title, text);
            
            AddCategoryCriteria(listSearchCriteriaContracts, selectedBookIds, selectedCategoryIds);

            using (var client = GetRestClient())
            {
                var request = new SearchRequestContract
                {
                    ConditionConjunction = listSearchCriteriaContracts,
                    Start = start,
                    Count = count,
                    Sort = (SortTypeEnumContract)sortingEnum,
                    SortDirection = sortAsc ? SortDirectionEnumContract.Asc : SortDirectionEnumContract.Desc,
                };
                var results = client.SearchAudioBook(request);
                return Json(new { books = results }, GetJsonSerializerSettingsForBiblModule());
            }
        }

        public FileResult DownloadAudio(long audioId)
        {
            using (var client = GetRestClient())
            {
                var fileData = client.GetAudio(audioId);
                Response.ContentLength = fileData.FileSize;
                return File(fileData.Stream, fileData.MimeType, fileData.FileName);
            }
        }
    }
}