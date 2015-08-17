using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;
using AutoMapper;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Searching.Criteria;
using ITJakub.Web.Hub.Converters;
using ITJakub.Web.Hub.Models;
using ITJakub.Web.Hub.Models.Plugins.RegExSearch;
using Newtonsoft.Json;

namespace ITJakub.Web.Hub.Areas.BohemianTextBank.Controllers
{
    [RouteArea("BohemianTextBank")]
    public class BohemianTextBankController : Controller
    {

        private readonly ItJakubServiceClient m_serviceClient = new ItJakubServiceClient();

        public ActionResult Index()
        {
            return View("List");
        }

        public ActionResult Search()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        public ActionResult Information()
        {
            return View();
        }

        public ActionResult Feedback()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Feedback(FeedbackViewModel model)
        {
            var client = new ItJakubServiceClient();
            client.CreateAnonymousFeedback(model.Text, model.Name, model.Email);
            return View("Information");
        }

        public ActionResult Help()
        {
            return View();
        }

        public ActionResult TextSearchCount(string text, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var listSearchCriteriaContracts = new List<SearchCriteriaContract>
            {
                new WordListCriteriaContract
                {
                  Key = CriteriaKey.Fulltext,
                  Disjunctions = new List<WordCriteriaContract>
                  {
                      new WordCriteriaContract
                      {
                          Contains = new List<string>{ text }
                      }
                  }
                }
            };

            if (selectedBookIds != null || selectedCategoryIds != null)
            {
                listSearchCriteriaContracts.Add(new SelectedCategoryCriteriaContract
                {
                    SelectedBookIds = selectedBookIds,
                    SelectedCategoryIds = selectedCategoryIds
                });
            }

            var count = m_serviceClient.GetCorpusSearchResultsCount(listSearchCriteriaContracts);

            return Json(new { count }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TextSearchPaged(string text, int start, int count, int contextLength, short sortingEnum, bool sortAsc, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var listSearchCriteriaContracts = new List<SearchCriteriaContract>
            {
                new WordListCriteriaContract
                {
                  Key = CriteriaKey.Fulltext,
                  Disjunctions = new List<WordCriteriaContract>
                  {
                      new WordCriteriaContract
                      {
                          Contains = new List<string>{ text }
                      }
                  }
                },
                new ResultCriteriaContract
                {
                    Sorting = (SortEnum) sortingEnum,
                    Direction = sortAsc ? ListSortDirection.Ascending : ListSortDirection.Descending,
                    HitSettingsContract = new HitSettingsContract
                    {
                        ContextLength = contextLength,
                        Count = count,
                        Start = start
                    }
                }
            };

            if (selectedBookIds != null || selectedCategoryIds != null)
            {
                listSearchCriteriaContracts.Add(new SelectedCategoryCriteriaContract
                {
                    SelectedBookIds = selectedBookIds,
                    SelectedCategoryIds = selectedCategoryIds
                });
            }

            var results = m_serviceClient.GetCorpusSearchResults(listSearchCriteriaContracts);
            return Json(new { results = results.SearchResults }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AdvancedSearchResultsCount(string json, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var deserialized = JsonConvert.DeserializeObject<IList<ConditionCriteriaDescriptionBase>>(json, new ConditionCriteriaDescriptionConverter());
            var listSearchCriteriaContracts = Mapper.Map<IList<SearchCriteriaContract>>(deserialized);

            if (selectedBookIds != null || selectedCategoryIds != null)
            {
                listSearchCriteriaContracts.Add(new SelectedCategoryCriteriaContract
                {
                    SelectedBookIds = selectedBookIds,
                    SelectedCategoryIds = selectedCategoryIds
                });
            }

            var count = m_serviceClient.GetCorpusSearchResultsCount(listSearchCriteriaContracts);
            return Json(new { count }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AdvancedSearchPaged(string json, int start, int count,int contextLength, short sortingEnum, bool sortAsc, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var deserialized = JsonConvert.DeserializeObject<IList<ConditionCriteriaDescriptionBase>>(json, new ConditionCriteriaDescriptionConverter());
            var listSearchCriteriaContracts = Mapper.Map<IList<SearchCriteriaContract>>(deserialized);

            listSearchCriteriaContracts.Add(new ResultCriteriaContract
            {
                Sorting = (SortEnum)sortingEnum,
                Direction = sortAsc ? ListSortDirection.Ascending : ListSortDirection.Descending,
                HitSettingsContract = new HitSettingsContract
                {
                    ContextLength = contextLength,
                    Count = count,
                    Start = start
                }
            });

            if (selectedBookIds != null || selectedCategoryIds != null)
            {
                listSearchCriteriaContracts.Add(new SelectedCategoryCriteriaContract
                {
                    SelectedBookIds = selectedBookIds,
                    SelectedCategoryIds = selectedCategoryIds
                });
            }

            var results = m_serviceClient.GetCorpusSearchResults(listSearchCriteriaContracts);
            return Json(new { results = results.SearchResults }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTypeaheadAuthor(string query)
        {
            var result = m_serviceClient.GetTypeaheadAuthorsByBookType(query, BookTypeEnumContract.TextBank);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTypeaheadTitle(string query)
        {
            var result = m_serviceClient.GetTypeaheadTitlesByBookType(query, BookTypeEnumContract.TextBank, null, null);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}