using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ITJakub.Shared.Contracts.Notes;
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
using Vokabular.Shared.DataContracts.Search.Corpus;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Search.CriteriaItem;
using Vokabular.Shared.DataContracts.Search.OldCriteriaItem;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.Web.Hub.Areas.ProfessionalLiterature.Controllers
{
    [Area("ProfessionalLiterature")]
    public class ProfessionalLiteratureController : AreaController
    {
        private readonly StaticTextManager m_staticTextManager;
        private readonly FeedbacksManager m_feedbacksManager;

        public ProfessionalLiteratureController(StaticTextManager staticTextManager, FeedbacksManager feedbacksManager, CommunicationProvider communicationProvider) : base(communicationProvider)
        {
            m_staticTextManager = staticTextManager;
            m_feedbacksManager = feedbacksManager;
        }

        private FeedbackFormIdentification GetFeedbackFormIdentification()
        {
            return new FeedbackFormIdentification { Area = "ProfessionalLiterature", Controller = "ProfessionalLiterature" };
        }

        protected override BookTypeEnumContract AreaBookType => BookTypeEnumContract.ProfessionalLiterature;

        public ActionResult Index()
        {
            return View("List");
        }

        public ActionResult Search()
        {
            return View();
        }

        public ActionResult Listing(long bookId, string searchText, string page)
        {
            // TODO modify this method according to EditionsController.Listing()

            using (var client = GetRestClient())
            {
                var book = client.GetBookInfo(bookId);
                var pages = client.GetBookPageList(bookId);

                return
                    View(new BookListingModel
                    {
                        BookId = book.Id,
                        BookXmlId = book.Id.ToString(),
                        VersionXmlId = null,
                        BookTitle = book.Title,
                        BookPages = pages,
                        SearchText = searchText,
                        InitPageXmlId = page,
                        JsonSerializerSettingsForBiblModule = GetJsonSerializerSettingsForBiblModule()
                    });
            }
        }

        public ActionResult GetListConfiguration()
        {
            var fullPath = "~/Areas/ProfessionalLiterature/content/BibliographyPlugin/list_configuration.json";
            return File(fullPath, "application/json", fullPath);
        }

        public ActionResult GetSearchConfiguration()
        {
            var fullPath = "~/Areas/ProfessionalLiterature/content/BibliographyPlugin/search_configuration.json";
            return File(fullPath, "application/json", fullPath);
        }

        public FileResult GetBookImage(string bookId, int position)
        {
            using (var client = GetMainServiceClient())
            {
                var imageDataStream = client.GetBookPageImage(bookId, position);
                return new FileStreamResult(imageDataStream, "image/jpeg"); //TODO resolve content type properly
            }
        }

        public ActionResult List()
        {
            return View();
        }

        public ActionResult Information()
        {
            var pageStaticText = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextProfessionalInfo);
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

            m_feedbacksManager.CreateFeedback(model, FeedbackCategoryEnumContract.ProfessionalLiterature, GetMainServiceClient(), IsUserLoggedIn(), GetUserName());
            return View("Feedback/FeedbackSuccess");
        }

        public ActionResult Help()
        {
            var pageStaticText = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextProfessionalHelp);
            return View(pageStaticText);
        }

        public ActionResult GetProfessionalLiteratureWithCategories()
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
            var result = SearchByCriteriaJson(json, start, count, sortingEnum, sortAsc, selectedBookIds, selectedCategoryIds);
            return Json(new { books = result }, GetJsonSerializerSettingsForBiblModule());
        }

        public ActionResult TextSearchCount(string text, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var count = SearchByCriteriaTextCount(CriteriaKey.Title, text, selectedBookIds, selectedCategoryIds);
            return Json(new { count });
        }

        public ActionResult TextSearchFulltextCount(string text, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var count = SearchByCriteriaTextCount(CriteriaKey.Fulltext, text, selectedBookIds, selectedCategoryIds);
            return Json(new { count });
        }

        public ActionResult TextSearchPaged(string text, int start, int count, short sortingEnum, bool sortAsc, IList<long> selectedBookIds,
            IList<int> selectedCategoryIds)
        {
            var result = SearchByCriteriaText(CriteriaKey.Title, text, start, count, sortingEnum, sortAsc, selectedBookIds, selectedCategoryIds);
            return Json(new { books = result }, GetJsonSerializerSettingsForBiblModule());
        }

        public ActionResult TextSearchFulltextPaged(string text, int start, int count, short sortingEnum, bool sortAsc, IList<long> selectedBookIds,
            IList<int> selectedCategoryIds)
        {
            var result = SearchByCriteriaText(CriteriaKey.Fulltext, text, start, count, sortingEnum, sortAsc, selectedBookIds, selectedCategoryIds);
            return Json(new { books = result }, GetJsonSerializerSettingsForBiblModule());
        }

        #region search in book

        public ActionResult AdvancedSearchInBookPaged(string json, int start, int count, string bookXmlId, string versionXmlId)
        {
            var deserialized = JsonConvert.DeserializeObject<IList<ConditionCriteriaDescriptionBase>>(json, new ConditionCriteriaDescriptionConverter());
            var listSearchCriteriaContracts = Mapper.Map<IList<SearchCriteriaContract>>(deserialized);

            listSearchCriteriaContracts.Add(new ResultCriteriaContract
            {
                Start = 0,
                Count = 1,
                HitSettingsContract = new HitSettingsContract
                {
                    ContextLength = 45,
                    Count = count,
                    Start = start
                }
            });

            listSearchCriteriaContracts.Add(new ResultRestrictionCriteriaContract
            {
                ResultBooks = new List<BookVersionPairContract> {new BookVersionPairContract {Guid = bookXmlId, VersionId = versionXmlId}}
            });
            using (var client = GetMainServiceClient())
            {
                var result = client.SearchByCriteria(listSearchCriteriaContracts).FirstOrDefault();
                if (result != null)
                {
                    return Json(new {results = result.Results}, GetJsonSerializerSettingsForBiblModule());
                }

                return Json(new {results = new PageResultContext[0]}, GetJsonSerializerSettingsForBiblModule());
            }
        }

        public ActionResult AdvancedSearchInBookCount(string json, string bookXmlId, string versionXmlId)
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
                ResultBooks = new List<BookVersionPairContract> {new BookVersionPairContract {Guid = bookXmlId, VersionId = versionXmlId}}
            });
            using (var client = GetMainServiceClient())
            {
                var result = client.SearchByCriteria(listSearchCriteriaContracts).FirstOrDefault();
                if (result != null)
                {
                    var count = result.TotalHitCount;
                    return Json(new {count});
                }

                return Json(new {count = 0});
            }
        }

        public ActionResult AdvancedSearchInBookPagesWithMatchHit(string json, string bookXmlId, string versionXmlId)
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
                ResultBooks = new List<BookVersionPairContract> {new BookVersionPairContract {Guid = bookXmlId, VersionId = versionXmlId}}
            });
            using (var client = GetMainServiceClient())
            {
                var result = client.GetSearchEditionsPageList(listSearchCriteriaContracts);     //TODO rename to searchBookPageList (inlcuding xquery)

                return Json(new {pages = result.PageList}, GetJsonSerializerSettingsForBiblModule());
            }
        }

        public ActionResult TextSearchInBookPaged(string text, int start, int count, string bookXmlId, string versionXmlId)
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
                            Contains = new List<string> {text}
                        }
                    }
                },
                new ResultCriteriaContract
                {
                    Start = 0,
                    Count = 1,
                    HitSettingsContract = new HitSettingsContract
                    {
                        ContextLength = 45,
                        Count = count,
                        Start = start
                    }
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
            using (var client = GetMainServiceClient())
            {
                var result = client.SearchByCriteria(listSearchCriteriaContracts).FirstOrDefault();
                if (result != null)
                {
                    return Json(new {results = result.Results}, GetJsonSerializerSettingsForBiblModule());
                }

                return Json(new {results = new PageResultContext[0]}, GetJsonSerializerSettingsForBiblModule());
            }
        }

        public ActionResult TextSearchInBookCount(string text, string bookXmlId, string versionXmlId)
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
                            Contains = new List<string> {text}
                        }
                    }
                },
                new ResultCriteriaContract
                {
                    Start = 0,
                    Count = 1
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
            using (var client = GetMainServiceClient())
            {
                var result = client.SearchByCriteria(listSearchCriteriaContracts).FirstOrDefault();
                if (result != null)
                {
                    var count = result.TotalHitCount;
                    return Json(new {count});
                }

                return Json(new {count = 0});
            }
        }

        public ActionResult TextSearchInBookPagesWithMatchHit(string text, string bookXmlId, string versionXmlId)
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
                            Contains = new List<string> {text}
                        }
                    }
                },
                new ResultCriteriaContract
                {
                    Start = 0,
                    Count = 1
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
            using (var client = GetMainServiceClient())
            {
                var result = client.GetSearchEditionsPageList(listSearchCriteriaContracts);

                return Json(new {pages = result.PageList}, GetJsonSerializerSettingsForBiblModule());
            }
        }

        #endregion
    }
}