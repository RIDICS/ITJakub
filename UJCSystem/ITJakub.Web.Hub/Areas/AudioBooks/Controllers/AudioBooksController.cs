using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;
using AutoMapper;
using ITJakub.ITJakubService.DataContracts.Contracts.AudioBooks;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Notes;
using ITJakub.Shared.Contracts.Searching.Criteria;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Converters;
using ITJakub.Web.Hub.Managers;
using ITJakub.Web.Hub.Models;
using ITJakub.Web.Hub.Models.Plugins.RegExSearch;
using Newtonsoft.Json;

namespace ITJakub.Web.Hub.Areas.AudioBooks.Controllers
{
    [RouteArea("AudioBooks")]
    public class AudioBooksController : AreaController
    {
        private readonly StaticTextManager m_staticTextManager;
        private readonly FeedbacksManager m_feedbacksManager;

        public AudioBooksController(StaticTextManager staticTextManager, FeedbacksManager feedbacksManager)
        {
            m_staticTextManager = staticTextManager;
            m_feedbacksManager = feedbacksManager;
        }

        public override BookTypeEnumContract AreaBookType
        {
            get { return BookTypeEnumContract.AudioBook; }
        }

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
            var pageStaticText = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextAudioBooksInfo);
            return View(pageStaticText);
        }

        public ActionResult Feedback()
        {
            var viewModel = m_feedbacksManager.GetBasicViewModel(GetFeedbackFormIdentification(), StaticTexts.TextHomeFeedback, GetEncryptedClient(), GetUserName());
            return View(viewModel);

            //var pageStaticText = m_staticTextManager.GetRenderedHtmlText(StaticTexts.TextHomeFeedback);

            //var username = HttpContext.User.Identity.Name;
            //if (string.IsNullOrWhiteSpace(username))
            //{
            //    return View(new FeedbackViewModel
            //    {
            //        PageStaticText = pageStaticText
            //    });
            //}
            //using (var client = GetEncryptedClient())
            //{
            //    var user = client.FindUserByUserName(username);
            //    var viewModel = new FeedbackViewModel
            //    {
            //        Name = string.Format("{0} {1}", user.FirstName, user.LastName),
            //        Email = user.Email,
            //        PageStaticText = pageStaticText
            //    };

            //    return View(viewModel);
            //}
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

            m_feedbacksManager.CreateFeedback(model, FeedbackCategoryEnumContract.AudioBooks, GetMainServiceClient(), Request.IsAuthenticated, GetUserName());
            return View("Feedback/FeedbackSuccess");
        }

        public ActionResult List()
        {
            return View();
        }


        public ActionResult GetTypeaheadAuthor(string query)
        {
            using (var client = GetMainServiceClient())
            {
                var result = client.GetTypeaheadAuthorsByBookType(query, AreaBookType);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetTypeaheadTitle(IList<int> selectedCategoryIds, IList<long> selectedBookIds, string query)
        {
            using (var client = GetMainServiceClient())
            {
                var result = client.GetTypeaheadTitlesByBookType(query, AreaBookType, selectedCategoryIds, selectedBookIds);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetAudioWithCategories()
        {
            using (var client = GetMainServiceClient())
            {
                var audiosWithCategories = client.GetBooksWithCategoriesByBookType(AreaBookType);
                return Json(audiosWithCategories, JsonRequestBehavior.AllowGet);
            }
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

            using (var client = GetMainServiceClient())
            {
                var count = client.GetAudioBooksSearchResultsCount(listSearchCriteriaContracts);
                return Json(new {count}, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AdvancedSearchPaged(string json, int start, int count, short sortingEnum, bool sortAsc, IList<long> selectedBookIds,
            IList<int> selectedCategoryIds)
        {
            var deserialized = JsonConvert.DeserializeObject<IList<ConditionCriteriaDescriptionBase>>(json, new ConditionCriteriaDescriptionConverter());
            var listSearchCriteriaContracts = Mapper.Map<IList<SearchCriteriaContract>>(deserialized);

            listSearchCriteriaContracts.Add(new ResultCriteriaContract
            {
                Start = start,
                Count = count,
                Sorting = (SortEnum) sortingEnum,
                Direction = sortAsc ? ListSortDirection.Ascending : ListSortDirection.Descending
            });

            if (selectedBookIds != null || selectedCategoryIds != null)
            {
                listSearchCriteriaContracts.Add(new SelectedCategoryCriteriaContract
                {
                    SelectedBookIds = selectedBookIds,
                    SelectedCategoryIds = selectedCategoryIds
                });
            }

            using (var client = GetMainServiceClient())
            {
                var results = client.GetAudioBooksSearchResults(listSearchCriteriaContracts);
                return Json(new {books = results.Results}, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult TextSearchCount(string text, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var listSearchCriteriaContracts = new List<SearchCriteriaContract>();

            if(!string.IsNullOrEmpty(text))
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

            if (selectedBookIds != null || selectedCategoryIds != null)
            {
                listSearchCriteriaContracts.Add(new SelectedCategoryCriteriaContract
                {
                    SelectedBookIds = selectedBookIds,
                    SelectedCategoryIds = selectedCategoryIds
                });
            }

            using (var client = GetMainServiceClient())
            {
                var count = client.GetAudioBooksSearchResultsCount(listSearchCriteriaContracts);

                return Json(new {count}, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult TextSearchPaged(string text, int start, int count, short sortingEnum, bool sortAsc, IList<long> selectedBookIds,
            IList<int> selectedCategoryIds)
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

            if (selectedBookIds != null || selectedCategoryIds != null)
            {
                listSearchCriteriaContracts.Add(new SelectedCategoryCriteriaContract
                {
                    SelectedBookIds = selectedBookIds,
                    SelectedCategoryIds = selectedCategoryIds
                });
            }

            using (var client = GetMainServiceClient())
            {
                var results = client.GetAudioBooksSearchResults(listSearchCriteriaContracts);
                return Json(new {books = results.Results}, JsonRequestBehavior.AllowGet);
            }
        }

        public FileResult DownloadAudioBookTrack(long bookId, int trackPosition, AudioTypeContract audioType)
        {
            var audioTrackContract = new DownloadAudioBookTrackContract
            {
                BookId = bookId,
                RequestedAudioType = audioType,
                TrackPosition = trackPosition
            };

            using (var client = GetStreamingClient())
            {
                var audioTrack = client.DownloadAudioBookTrack(audioTrackContract);
                var result = new FileStreamResult(audioTrack.FileData, audioTrack.MimeType) {FileDownloadName = audioTrack.FileName};
                return result;
            }
        }

        public FileResult DownloadAudioBook(long bookId, AudioTypeContract audioType)
        {
            var audioTrackContract = new DownloadWholeBookContract
            {
                BookId = bookId,
                RequestedAudioType = audioType
            };
            using (var client = GetStreamingClient())
            {
                var audioTrack = client.DownloadWholeAudiobook(audioTrackContract);
                var result = new FileStreamResult(audioTrack.FileData, audioTrack.MimeType) {FileDownloadName = audioTrack.FileName};
                return result;
            }
        }
    }
}