using System.Collections.Generic;
using System.Web.Mvc;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Searching;
using ITJakub.Web.Hub.Areas.Editions.Models;
using Microsoft.Ajax.Utilities;

namespace ITJakub.Web.Hub.Areas.Editions.Controllers
{
    [RouteArea("Editions")]
    public class EditionsController : Controller
    {
        private readonly ItJakubServiceClient m_serviceClient;

        public EditionsController()
        {
            m_serviceClient = new ItJakubServiceClient();
        }

        // GET: Editions/Editions
        public ActionResult Index()
        {
            return View("Information");
        }

        public ActionResult Search()
        {
            return View();
        }

        public ActionResult SearchEditions(string term)
        {
            IEnumerable<SearchResultContract> listBooks = term.IsNullOrWhiteSpace() ? m_serviceClient.GetBooksByBookType(BookTypeEnumContract.Edition) : m_serviceClient.SearchBooksWithBookType(term, BookTypeEnumContract.Edition);
            
            foreach (var list in listBooks)
            {
                list.CreateTimeString = list.CreateTime.ToString();
            }
            return Json(new { books = listBooks }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Listing(string bookId)
        {
            var book = m_serviceClient.GetBookInfo(bookId);
            return View(new BookListingModel { BookId = book.Guid, BookTitle = book.Title, BookPages = book.BookPages});
        }


        public FileResult GetBookImage(string bookId, int position)
        {
            var imageDataStream = m_serviceClient.GetBookPageImage(new BookPageImageContract
            {
                BookGuid = bookId,
                Position = position
            });
            return new FileStreamResult(imageDataStream, "image/jpeg"); //TODO resolve content type properly
        }

        public ActionResult List()
        {
            return View();
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

        public ActionResult GetTypeaheadAuthor(string query)
        {
            var result = m_serviceClient.GetTypeaheadAuthorsByBookType(query, BookTypeEnumContract.Edition);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTypeaheadTitle(string query)
        {
            var result = m_serviceClient.GetTypeaheadTitlesByBookType(query, BookTypeEnumContract.Edition);
            return Json(result, JsonRequestBehavior.AllowGet);
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
                        EndsWith = "Romanorum_",
                        StartsWith = "_Gesta"
                    },
                    new WordCriteriaContract
                    {
                        StartsWith = "[Sbírka",
                        Contains = new List<string> {"založených", "na"},
                        EndsWith = "legendách]"
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

            var wordListCriteriaContracts = new List<WordListCriteriaContract> { title1, title2, editor1, editor2, fulltext1, fulltext2, sentence1, sentence2, heading1, heading2 };
            m_serviceClient.SearchByCriteria(wordListCriteriaContracts);
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }
    }
}