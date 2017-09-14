using System.Collections.Generic;
using System.ComponentModel;
using AutoMapper;
using ITJakub.Shared.Contracts.Notes;
using ITJakub.Shared.Contracts.Searching.Criteria;
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
using Vokabular.Shared.DataContracts.Search.CriteriaItem;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.Web.Hub.Areas.Bibliographies.Controllers
{
    [Area("Bibliographies")]
    public class BibliographiesController : AreaController
    {
        private readonly StaticTextManager m_staticTextManager;
        private readonly FeedbacksManager m_feedbacksManager;

        public BibliographiesController(StaticTextManager staticTextManager, FeedbacksManager feedbacksManager, CommunicationProvider communicationProvider) : base(communicationProvider)
        {
            m_staticTextManager = staticTextManager;
            m_feedbacksManager = feedbacksManager;
        }

        public override BookTypeEnumContract AreaBookType => BookTypeEnumContract.BibliographicalItem;
        
        private FeedbackFormIdentification GetFeedbackFormIdentification()
        {
            return new FeedbackFormIdentification { Area = "Bibliographies", Controller = "Bibliographies" };
        }

        public ActionResult Index()
        {
            return View("Search");
        }

        public ActionResult Search()
        {
            return View();
        }

        public ActionResult Information()
        {
            var pageStaticText = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextBibliographiesInfo);
            return View(pageStaticText);
        }

        public ActionResult Feedback()
        {
            var viewModel = m_feedbacksManager.GetBasicViewModel(GetFeedbackFormIdentification(), StaticTexts.TextHomeFeedback, GetEncryptedClient(), GetUserName());
            return View(viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Feedback(FeedbackViewModel model)
        {
            if (!ModelState.IsValid)
            {
                m_feedbacksManager.FillViewModel(model, StaticTexts.TextHomeFeedback, GetFeedbackFormIdentification());
                return View(model);
            }

            m_feedbacksManager.CreateFeedback(model, FeedbackCategoryEnumContract.Bibliographies, GetMainServiceClient(), IsUserLoggedIn(), GetUserName());
            return View("Feedback/FeedbackSuccess");
        }

        public ActionResult GetTypeaheadAuthor(string query)
        {
            using (var client = GetMainServiceClient())
            {
                var result = client.GetTypeaheadAuthors(query);
                return Json(result);
            }
        }

        public ActionResult GetTypeaheadTitle(string query)
        {
            using (var client = GetMainServiceClient())
            {
                var result = client.GetTypeaheadTitles(query);
                return Json(result);
            }
        }

        public ActionResult AdvancedSearchResultsCount(string json)
        {
            var deserialized = JsonConvert.DeserializeObject<IList<ConditionCriteriaDescriptionBase>>(json, new ConditionCriteriaDescriptionConverter());
            var listSearchCriteriaContracts = Mapper.Map<IList<SearchCriteriaContract>>(deserialized);

            using (var client = GetMainServiceClient())
            {
                var count = client.SearchCriteriaResultsCount(listSearchCriteriaContracts);
                return Json(new { count });
            }
        }

        public ActionResult AdvancedSearchPaged(string json, int start, int count, short sortingEnum, bool sortAsc)
        {
            var deserialized = JsonConvert.DeserializeObject<IList<ConditionCriteriaDescriptionBase>>(json, new ConditionCriteriaDescriptionConverter());
            var listSearchCriteriaContracts = Mapper.Map<IList<SearchCriteriaContract>>(deserialized);

            listSearchCriteriaContracts.Add(new ResultCriteriaContract
            {
                Start = start,
                Count = count,
                Sorting = (SortEnum)sortingEnum,
                Direction = sortAsc ? ListSortDirection.Ascending : ListSortDirection.Descending
            });

            using (var client = GetMainServiceClient())
            {
                var results = client.SearchByCriteria(listSearchCriteriaContracts);
                return Json(new { results }, GetJsonSerializerSettingsForBiblModule());
            }
        }


        public ActionResult TextSearchCount(string text)
        {
            var listSearchCriteriaContracts = new List<SearchCriteriaContract>();

            if (!string.IsNullOrEmpty(text))
            {
                var wordListCriteria = new WordListCriteriaContract
                {
                    Key = CriteriaKey.Title,
                    Disjunctions = new List<WordCriteriaContract>
                    {
                        new WordCriteriaContract
                        {
                            Contains = new List<string> {text}
                        }
                    }
                };

                listSearchCriteriaContracts.Add(wordListCriteria);
            }

            using (var client = GetMainServiceClient())
            {
                var count = client.SearchCriteriaResultsCount(listSearchCriteriaContracts);

                return Json(new { count });
            }
        }


        public ActionResult TextSearchPaged(string text, int start, int count, short sortingEnum, bool sortAsc)
        {
            var listSearchCriteriaContracts = new List<SearchCriteriaContract>
            {
                new ResultCriteriaContract
                {
                    Start = start,
                    Count = count,
                    Sorting = (SortEnum) sortingEnum,
                    Direction = sortAsc ? ListSortDirection.Ascending : ListSortDirection.Descending
                }
            };

            if (!string.IsNullOrEmpty(text))
            {
                var wordListCriteria = new WordListCriteriaContract
                {
                    Key = CriteriaKey.Title,
                    Disjunctions = new List<WordCriteriaContract>
                    {
                        new WordCriteriaContract
                        {
                            Contains = new List<string> {text}
                        }
                    }
                };

                listSearchCriteriaContracts.Add(wordListCriteria);
            }
            
            using (var client = GetMainServiceClient())
            {
                var results = client.SearchByCriteria(listSearchCriteriaContracts);
                return Json(new { results }, GetJsonSerializerSettingsForBiblModule());
            }
        }
    }
}