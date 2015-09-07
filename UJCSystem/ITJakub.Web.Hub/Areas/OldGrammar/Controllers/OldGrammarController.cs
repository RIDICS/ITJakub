using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Notes;
using ITJakub.Shared.Contracts.Searching;
using ITJakub.Shared.Contracts.Searching.Criteria;
using ITJakub.Web.Hub.Converters;
using ITJakub.Web.Hub.Models;
using ITJakub.Web.Hub.Models.Plugins.RegExSearch;
using Newtonsoft.Json;

namespace ITJakub.Web.Hub.Areas.OldGrammar.Controllers
{
    [RouteArea("OldGrammar")]
    public class OldGrammarController : Controller
    {
        private readonly ItJakubServiceClient m_mainServiceClient = new ItJakubServiceClient();
        private readonly ItJakubServiceEncryptedClient m_mainServiceEncryptedClient = new ItJakubServiceEncryptedClient();

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
            var username = HttpContext.User.Identity.Name;
            if (string.IsNullOrWhiteSpace(username))
            {
                return View();
            }

            var user = m_mainServiceEncryptedClient.FindUserByUserName(username);
            var viewModel = new FeedbackViewModel
            {
                Name = string.Format("{0} {1}", user.FirstName, user.LastName),
                Email = user.Email
            };

            return View(viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Feedback(FeedbackViewModel model)
        {
            var username = HttpContext.User.Identity.Name;

            if (string.IsNullOrWhiteSpace(username))
                m_mainServiceClient.CreateAnonymousFeedback(model.Text, model.Name, model.Email, FeedbackCategoryEnumContract.OldGrammar);
            else
                m_mainServiceEncryptedClient.CreateFeedback(model.Text, username, FeedbackCategoryEnumContract.OldGrammar);

            return View("Information");
        }

        public ActionResult Help()
        {
            return View();
        }

        public ActionResult Listing(string bookId, string searchText, string page)
        {
            var book = m_mainServiceClient.GetBookInfoWithPages(bookId);
            return
                View(new BookListingModel
                {
                    BookId = book.BookId,
                    BookXmlId = book.BookXmlId,
                    VersionXmlId = book.LastVersionXmlId,
                    BookTitle = book.Title,
                    BookPages = book.BookPages,
                    SearchText = searchText,
                    InitPageXmlId = page
                });
        }

        public ActionResult GetTypeaheadTerm(IList<int> selectedCategoryIds, IList<long> selectedBookIds, string query)
        {
            var result = m_mainServiceClient.GetTypeaheadTermsByBookType(query, BookTypeEnumContract.Grammar, selectedCategoryIds, selectedBookIds);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTypeaheadAuthor(string query)
        {
            var result = m_mainServiceClient.GetTypeaheadAuthorsByBookType(query, BookTypeEnumContract.Grammar);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTypeaheadTitle(IList<int> selectedCategoryIds, IList<long> selectedBookIds, string query)
        {
            var result = m_mainServiceClient.GetTypeaheadTitlesByBookType(query, BookTypeEnumContract.Grammar, selectedCategoryIds, selectedBookIds);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetGrammarsWithCategories()
        {
            var editionsWithCategories = m_mainServiceClient.GetBooksWithCategoriesByBookType(BookTypeEnumContract.Grammar);
            return Json(editionsWithCategories, JsonRequestBehavior.AllowGet);
        }


        public ActionResult AdvancedSearchPaged(string json, int start, int count, short sortingEnum, bool sortAsc, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var deserialized = JsonConvert.DeserializeObject<IList<ConditionCriteriaDescriptionBase>>(json, new ConditionCriteriaDescriptionConverter());
            var listSearchCriteriaContracts = Mapper.Map<IList<SearchCriteriaContract>>(deserialized);

            listSearchCriteriaContracts.Add(new ResultCriteriaContract
            {
                Start = start,
                Count = count,
                Sorting = (SortEnum)sortingEnum,
                Direction = sortAsc ? ListSortDirection.Ascending : ListSortDirection.Descending,
                HitSettingsContract = new HitSettingsContract
                {
                    ContextLength = 50,
                    Count = 3,
                    Start = 1
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

            if (listSearchCriteriaContracts.FirstOrDefault(x => x.Key == CriteriaKey.Term) != null)
            {
                listSearchCriteriaContracts.OfType<ResultCriteriaContract>().First().TermsSettingsContract = new TermsSettingsContract();
            }

            var result = m_mainServiceClient.SearchByCriteria(listSearchCriteriaContracts);
            return Json(new { books = result}, JsonRequestBehavior.AllowGet);
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

            var count = m_mainServiceClient.SearchCriteriaResultsCount(listSearchCriteriaContracts);
            return Json(new { count }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TextSearchCount(string text, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var listSearchCriteriaContracts = new List<SearchCriteriaContract>
            {
                new WordListCriteriaContract
                {
                    Key = CriteriaKey.Title,
                    Disjunctions = new List<WordCriteriaContract>
                    {
                        new WordCriteriaContract
                        {
                            Contains = new List<string> {text}
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

            var count = m_mainServiceClient.SearchCriteriaResultsCount(listSearchCriteriaContracts);

            return Json(new { count }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TextSearchFulltextCount(string text, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var listSearchCriteriaContracts = new List<SearchCriteriaContract>
            {
                new WordListCriteriaContract
                {
                    Key = CriteriaKey.Term,
                    Disjunctions = new List<WordCriteriaContract>
                    {
                        new WordCriteriaContract
                        {
                            Contains = new List<string> {text}
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

            var count = m_mainServiceClient.SearchCriteriaResultsCount(listSearchCriteriaContracts);

            return Json(new { count }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TextSearchPaged(string text, int start, int count, short sortingEnum, bool sortAsc, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var listSearchCriteriaContracts = new List<SearchCriteriaContract>
            {
                new WordListCriteriaContract
                {
                    Key = CriteriaKey.Title,
                    Disjunctions = new List<WordCriteriaContract>
                    {
                        new WordCriteriaContract
                        {
                            Contains = new List<string> {text}
                        }
                    }
                },
                new ResultCriteriaContract
                {
                    Start = start,
                    Count = count,
                    Sorting = (SortEnum) sortingEnum,
                    Direction = sortAsc ? ListSortDirection.Ascending : ListSortDirection.Descending,
                    HitSettingsContract = new HitSettingsContract
                    {
                        ContextLength = 50,
                        Count = 3,
                        Start = 1
                    }
                },
            };

            if (selectedBookIds != null || selectedCategoryIds != null)
            {
                listSearchCriteriaContracts.Add(new SelectedCategoryCriteriaContract
                {
                    SelectedBookIds = selectedBookIds,
                    SelectedCategoryIds = selectedCategoryIds
                });
            }

            var result = m_mainServiceClient.SearchByCriteria(listSearchCriteriaContracts);
            return Json(new { books = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TextSearchFulltextPaged(string text, int start, int count, short sortingEnum, bool sortAsc, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var listSearchCriteriaContracts = new List<SearchCriteriaContract>
            {
                new WordListCriteriaContract
                {
                    Key = CriteriaKey.Term,
                    Disjunctions = new List<WordCriteriaContract>
                    {
                        new WordCriteriaContract
                        {
                            Contains = new List<string> {text}
                        }
                    }
                },
                new ResultCriteriaContract
                {
                    Start = start,
                    Count = count,
                    Sorting = (SortEnum) sortingEnum,
                    Direction = sortAsc ? ListSortDirection.Ascending : ListSortDirection.Descending,
                    HitSettingsContract = new HitSettingsContract
                    {
                        ContextLength = 50,
                        Count = 3,
                        Start = 1
                    },
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

            if (!string.IsNullOrWhiteSpace(text))
            {
                listSearchCriteriaContracts.OfType<ResultCriteriaContract>().First().TermsSettingsContract = new TermsSettingsContract();
            }

            var result = m_mainServiceClient.SearchByCriteria(listSearchCriteriaContracts);
            return Json(new { books = result }, JsonRequestBehavior.AllowGet);
        }



        #region search in boook

        public ActionResult TextSearchInBook(string text, string bookXmlId, string versionXmlId)
        {
            var listSearchCriteriaContracts = new List<SearchCriteriaContract>
            {
                new WordListCriteriaContract
                {
                    Key = CriteriaKey.Term,
                    Disjunctions = new List<WordCriteriaContract>
                    {
                        new WordCriteriaContract
                        {
                            Contains = new List<string> {text}
                        }
                    }
                },
                new ResultCriteriaContract
                {
                    Start = 0,
                    Count = 1,
                },
                new ResultRestrictionCriteriaContract
                {
                    ResultBooks =
                        new List<BookVersionPairContract>
                        {
                            new BookVersionPairContract {Guid = bookXmlId, VersionId = versionXmlId}
                        }
                }
            };

            if (!string.IsNullOrWhiteSpace(text))
            {
                listSearchCriteriaContracts.OfType<ResultCriteriaContract>().First().TermsSettingsContract = new TermsSettingsContract();
            }

            var result = m_mainServiceClient.SearchByCriteria(listSearchCriteriaContracts).FirstOrDefault();
            if (result != null)
            {
                return Json(new { results = result.TermsPageHits }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AdvancedSearchInBook(string json, string bookXmlId, string versionXmlId)
        {
            var deserialized = JsonConvert.DeserializeObject<IList<ConditionCriteriaDescriptionBase>>(json, new ConditionCriteriaDescriptionConverter());
            var listSearchCriteriaContracts = Mapper.Map<IList<SearchCriteriaContract>>(deserialized);

            listSearchCriteriaContracts.Add(new ResultCriteriaContract
            {
                Start = 0,
                Count = 1
            });

            listSearchCriteriaContracts.Add(new ResultRestrictionCriteriaContract
            {
                ResultBooks = new List<BookVersionPairContract> { new BookVersionPairContract { Guid = bookXmlId, VersionId = versionXmlId } }
            });

            if (!string.IsNullOrWhiteSpace(json))
            {
                listSearchCriteriaContracts.OfType<ResultCriteriaContract>().First().TermsSettingsContract = new TermsSettingsContract();
            }

            var result = m_mainServiceClient.SearchByCriteria(listSearchCriteriaContracts).FirstOrDefault();
            if (result != null)
            {
                return Json(new { results = result.TermsPageHits }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}