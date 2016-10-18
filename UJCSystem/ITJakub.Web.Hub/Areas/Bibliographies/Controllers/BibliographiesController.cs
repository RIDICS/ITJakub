using System.Collections.Generic;
using System.ComponentModel;
using AutoMapper;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Notes;
using ITJakub.Shared.Contracts.Searching.Criteria;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Converters;
using ITJakub.Web.Hub.Managers;
using ITJakub.Web.Hub.Models;
using ITJakub.Web.Hub.Models.Plugins.RegExSearch;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ITJakub.Web.Hub.Areas.Bibliographies.Controllers
{
    [Area("Bibliographies")]
    public class BibliographiesController : AreaController
    {
        private readonly StaticTextManager m_staticTextManager;

        public BibliographiesController(StaticTextManager staticTextManager, CommunicationProvider communicationProvider) : base(communicationProvider)
        {
            m_staticTextManager = staticTextManager;
        }

        public override BookTypeEnumContract AreaBookType
        {
            get { return BookTypeEnumContract.BibliographicalItem; }
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
            var pageStaticText = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextHomeFeedback);

            var username = HttpContext.User.Identity.Name;
            if (string.IsNullOrWhiteSpace(username))
            {
                return View(new FeedbackViewModel
                {
                    PageStaticText = pageStaticText
                });
            }
            using (var client = GetEncryptedClient())
            {
                var user = client.FindUserByUserName(username);
                var viewModel = new FeedbackViewModel
                {
                    Name = string.Format("{0} {1}", user.FirstName, user.LastName),
                    Email = user.Email,
                    PageStaticText = pageStaticText
                };

                return View(viewModel);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Feedback(FeedbackViewModel model)
        {
            var username = HttpContext.User.Identity.Name;

            if (string.IsNullOrWhiteSpace(username))
            {
                using (var client = GetMainServiceClient())
                    client.CreateAnonymousFeedback(model.Text, model.Name, model.Email, FeedbackCategoryEnumContract.Bibliographies);
            }
            else
            {
                using (var client = GetMainServiceClient())
                {
                    client.CreateFeedback(model.Text, username, FeedbackCategoryEnumContract.Bibliographies);
                }
            }

            return View("Information");
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