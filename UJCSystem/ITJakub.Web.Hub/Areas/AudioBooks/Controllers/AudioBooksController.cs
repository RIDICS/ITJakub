using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;
using AutoMapper;
using ITJakub.ITJakubService.DataContracts.Clients;
using ITJakub.ITJakubService.DataContracts.Contracts.AudioBooks;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Notes;
using ITJakub.Shared.Contracts.Searching.Criteria;
using ITJakub.Web.Hub.Converters;
using ITJakub.Web.Hub.Models;
using ITJakub.Web.Hub.Models.Plugins.RegExSearch;
using Jewelery;
using Newtonsoft.Json;

namespace ITJakub.Web.Hub.Areas.AudioBooks.Controllers
{
    [RouteArea("AudioBooks")]
    public class AudioBooksController : Controller
    {

        //private readonly ItJakubServiceClient m_mainServiceClient = new ItJakubServiceClient();
        //private readonly ItJakubServiceEncryptedClient m_mainServiceEncryptedClient = new ItJakubServiceEncryptedClient();

        public ActionResult Index()
        {
            return View("List");
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
            using (var client = new ItJakubServiceEncryptedClient())
            {
                var user = client.FindUserByUserName(username);
                var viewModel = new FeedbackViewModel
                {
                    Name = string.Format("{0} {1}", user.FirstName, user.LastName),
                    Email = user.Email
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
                using (var client = new ItJakubServiceClient())
                {
                    client.CreateAnonymousFeedback(model.Text, model.Name, model.Email, FeedbackCategoryEnumContract.AudioBooks);
                }
            else
                using (var client = new ItJakubServiceEncryptedClient())
                {
                    client.CreateFeedback(model.Text, username, FeedbackCategoryEnumContract.AudioBooks);
                }

            return View("Information");
        }

        public ActionResult List()
        {
            return View();
        }


        public ActionResult GetTypeaheadAuthor(string query)
        {
            using (var client = new ItJakubServiceClient())
            {
                var result = client.GetTypeaheadAuthorsByBookType(query, BookTypeEnumContract.AudioBook);
                return Json(result, JsonRequestBehavior.AllowGet);
           }            
        }

        public ActionResult GetTypeaheadTitle(IList<int> selectedCategoryIds, IList<long> selectedBookIds, string query)
        {
            using (var client = new ItJakubServiceClient())
            {
                var result = client.GetTypeaheadTitlesByBookType(query, BookTypeEnumContract.AudioBook, selectedCategoryIds, selectedBookIds);
                return Json(result, JsonRequestBehavior.AllowGet);

            }
            
        }

        public ActionResult GetAudioWithCategories()
        {
            using (var client = new ItJakubServiceClient())
            {
                var audiosWithCategories = client.GetBooksWithCategoriesByBookType(BookTypeEnumContract.AudioBook);
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

            using (var client = new ItJakubServiceClient())
            {
                var count = client.GetAudioBooksSearchResultsCount(listSearchCriteriaContracts);
                return Json(new { count }, JsonRequestBehavior.AllowGet);
            }
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
            });

            if (selectedBookIds != null || selectedCategoryIds != null)
            {
                listSearchCriteriaContracts.Add(new SelectedCategoryCriteriaContract
                {
                    SelectedBookIds = selectedBookIds,
                    SelectedCategoryIds = selectedCategoryIds
                });
            }

            using (var client = new ItJakubServiceClient())
            {
                var results = client.GetAudioBooksSearchResults(listSearchCriteriaContracts);
                return Json(new { books = results.Results }, JsonRequestBehavior.AllowGet);
            }

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

            using (var client = new ItJakubServiceClient())
            {
                var count = client.GetAudioBooksSearchResultsCount(listSearchCriteriaContracts);

                return Json(new {count}, JsonRequestBehavior.AllowGet);
            }
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
                          Contains = new List<string>{ text }
                      }
                  }
                },
                new ResultCriteriaContract
                {
                    Start = start,
                    Count = count,
                    Sorting = (SortEnum) sortingEnum,
                    Direction = sortAsc ? ListSortDirection.Ascending : ListSortDirection.Descending,
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

            using (var client = new ItJakubServiceClient())
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

            using (var client = new ItJakubServiceStreamedClient())
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
                RequestedAudioType = audioType,
            };
            using (var client = new ItJakubServiceStreamedClient())
            {
                var audioTrack = client.DownloadWholeAudiobook(audioTrackContract);
                var result = new FileStreamResult(audioTrack.FileData, audioTrack.MimeType) {FileDownloadName = audioTrack.FileName};
                return result;
            }
        }
    }
}