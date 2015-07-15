using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.Shared.Contracts;
using ITJakub.Web.Hub.Models.Plugins.RegExSearch;
using Newtonsoft.Json;

namespace ITJakub.Web.Hub.Areas.Dictionaries.Controllers
{
    [RouteArea("Dictionaries")]
    public class DictionariesController : Controller
    {
        private readonly ItJakubServiceClient m_mainServiceClient;

        public DictionariesController()
        {
            m_mainServiceClient = new ItJakubServiceClient();
        }

        // GET: Dictionaries/Dictionaries
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Search()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        public ActionResult Headwords()
        {
            return View();
        }

        public ActionResult GetTextWithCategories()
        {
            var dictionariesAndCategories =
                m_mainServiceClient.GetBooksWithCategoriesByBookType(BookTypeEnumContract.Edition);
            //var booksDictionary =
            //    dictionariesAndCategories.Books.GroupBy(x => x.CategoryId)
            //        .ToDictionary(x => x.Key.ToString(), x => x.ToList());
            var categoriesDictionary =
                dictionariesAndCategories.Categories.GroupBy(x => x.ParentCategoryId)
                    .ToDictionary(x => x.Key == null ? "" : x.Key.ToString(), x => x.ToList());
            return
                Json(
                    new
                    {
                        type = BookTypeEnumContract.Edition,
                        //books = booksDictionary,
                        categories = categoriesDictionary
                    }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDictionariesWithCategories()
        {
            var dictionariesAndCategories =
                m_mainServiceClient.GetBooksWithCategoriesByBookType(BookTypeEnumContract.Dictionary);
            //TODO better fix loading books
            //var booksDictionary =
            //    dictionariesAndCategories.Books.GroupBy(x => x.CategoryId)
            //        .ToDictionary(x => x.Key.ToString(), x => x.ToList());
            var booksDictionary = new Dictionary<string, IList<BookContract>>();
            foreach (var book in dictionariesAndCategories.Books)
            {
                foreach (var categoryId in book.CategoryIds)
                {
                    IList<BookContract> booksInCategory;
                    if (booksDictionary.TryGetValue(Convert.ToString(categoryId), out booksInCategory))
                        booksInCategory.Add(book);
                    else
                        booksDictionary.Add(Convert.ToString(categoryId), new List<BookContract> {book});
                }
            }

            var categoriesDictionary =
                dictionariesAndCategories.Categories.GroupBy(x => x.ParentCategoryId)
                    .ToDictionary(x => x.Key == null ? "" : x.Key.ToString(), x => x.ToList());
            return
                Json(
                    new
                    {
                        type = BookTypeEnumContract.Dictionary,
                        books = booksDictionary,
                        categories = categoriesDictionary
                    }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Information()
        {
            return View();
        }

        public ActionResult TermsOfUse()
        {
            return View();
        }

        public ActionResult FeedBack()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SearchCriteria(string json)
        {
            var deserialized = JsonConvert.DeserializeObject<IList<ConditionCriteriaDescription>>(json, new ConditionCriteriaDescriptionConverter());
            var listSearchCriteriaContracts = Mapper.Map<IList<SearchCriteriaContract>>(deserialized);
            m_mainServiceClient.SearchByCriteria(listSearchCriteriaContracts);
            return Json(new { });

        }

        public ActionResult SearchCriteriaMocked()
        {
            var title1 = new WordListCriteriaContract
            {
                Key = CriteriaKey.Title,
                Values = new List<WordCriteriaContract>
                {
                    new WordCriteriaContract
                    {
                        EndsWith = "Romanoru_",
                        StartsWith = "_esta"
                    },
                    new WordCriteriaContract
                    {
                        StartsWith = "Sbírka",
                        Contains = new List<string> {"založených", "na"},
                        EndsWith = "legendách"
                    }
                }
            };


            var title2 = new WordListCriteriaContract
            {
                Key = CriteriaKey.Title,
                Values = new List<WordCriteriaContract>
                {
                    new WordCriteriaContract
                    {
                        Contains = new List<string> {"Ge%", "%oman", "Ge"}
                    },
                    new WordCriteriaContract
                    {
                        Contains = new List<string> {"al_žený"} //založených
                    },
                }
            };

            var editor1 = new WordListCriteriaContract
            {
                Key = CriteriaKey.Editor,
                Values = new List<WordCriteriaContract>
                {
                    new WordCriteriaContract
                    {
                        StartsWith = "Voleková"
                    },
                    new WordCriteriaContract
                    {
                        StartsWith = "Hanzová"
                    }
                }
            };


            var editor2 = new WordListCriteriaContract
            {
                Key = CriteriaKey.Editor,
                Values = new List<WordCriteriaContract>
                {
                    new WordCriteriaContract
                    {
                        Contains = new List<string> {"Kateřina"}
                    },
                    new WordCriteriaContract
                    {
                        Contains = new List<string> {"Barbora"}
                    },
                }
            };

            var fulltext1 = new WordListCriteriaContract
            {
                Key = CriteriaKey.Fulltext,
                Values = new List<WordCriteriaContract>
                {
                    //new WordCriteriaContract
                    //{
                    //    Contains = new List<string>{"T%s v Ř_mě"} // Titus v Římě    ---- Will this match if it's not single word?
                    //},
                    new WordCriteriaContract
                    {
                        Contains = new List<string> {"p%st_n%"} // pústeník
                    },
                }
            };

            var fulltext2 = new WordListCriteriaContract
            {
                Key = CriteriaKey.Fulltext,
                Values = new List<WordCriteriaContract>
                {
                    new WordCriteriaContract
                    {
                        StartsWith = "Ti",
                        Contains = new List<string> {"tu", "tus"},
                        EndsWith = "s"
                    },
                }
            };

            var sentence1 = new WordListCriteriaContract
            {
                Key = CriteriaKey.Sentence,
                Values = new List<WordCriteriaContract>
                {
                    new WordCriteriaContract
                    {
                        StartsWith = "Ti",
                        Contains = new List<string> {"tu", "tus"},
                        EndsWith = "s"
                    },
                    new WordCriteriaContract
                    {
                         Contains = new List<string> {"zavinil"}
                    }
                }
            };

            var sentence2 = new WordListCriteriaContract
            {
                Key = CriteriaKey.Sentence,
                Values = new List<WordCriteriaContract>
                {
                    new WordCriteriaContract
                    {
                        Contains = new List<string> {"prvorození"}
                    },
                }
            };

            var heading1 = new WordListCriteriaContract
            {
                Key = CriteriaKey.Heading,
                Values = new List<WordCriteriaContract>
                {
                    new WordCriteriaContract
                    {
                        Contains = new List<string> {"lenosti"}
                    },
                }
            };
            
            var heading2 = new WordListCriteriaContract
            {
                Key = CriteriaKey.Heading,
                Values = new List<WordCriteriaContract>
                {
                    new WordCriteriaContract
                    {
                        Contains = new List<string> {"synóv"}
                    },
                }
            };

            var wordListCriteriaContracts = new List<WordListCriteriaContract>{title1, title2, editor1, editor2, fulltext1, fulltext2, sentence1, sentence2, heading1, heading2};
            m_mainServiceClient.SearchByCriteria(wordListCriteriaContracts);
            return Json(new {}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetHeadwordDescription(string bookGuid, string xmlEntryId)
        {
            var result = m_mainServiceClient.GetDictionaryEntryByXmlId(bookGuid, xmlEntryId, OutputFormatEnumContract.Html);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetHeadwordCount(IList<int> selectedCategoryIds, IList<long> selectedBookIds)
        {
            var resultCount = m_mainServiceClient.GetHeadwordCount(selectedCategoryIds, selectedBookIds);
            return Json(resultCount, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetHeadwordList(IList<int> selectedCategoryIds, IList<long> selectedBookIds, int page, int pageSize)
        {
            var result = m_mainServiceClient.GetHeadwordList(selectedCategoryIds, selectedBookIds, page, pageSize);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetHeadwordPageNumber(IList<int> selectedCategoryIds, IList<long> selectedBookIds, string query, int pageSize)
        {
            var resultPageNumber = m_mainServiceClient.GetHeadwordPageNumber(selectedCategoryIds, selectedBookIds, query, pageSize);
            return Json(resultPageNumber, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSearchResultCount(string query)
        {
            //TODO search with category filter
            var result = m_mainServiceClient.GetHeadwordSearchResultCount(query);
            return Json(new {}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchHeadword(string query, int page, int pageSize)
        {
            //TODO
            var result = m_mainServiceClient.SearchHeadword(query, new List<string> { "{08BE3E56-77D0-46C1-80BB-C1346B757BE5}" }, page, pageSize);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTypeaheadDictionaryHeadword(string query)
        {
            var result = m_mainServiceClient.GetTypeaheadDictionaryHeadwords(query);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}