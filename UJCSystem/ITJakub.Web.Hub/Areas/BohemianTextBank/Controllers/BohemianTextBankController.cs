using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Searching.Criteria;

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
        public ActionResult Help()
        {
            return View();
        }

        public ActionResult GetMockedSearchResults() //TODO remove after test
        {
            var title1 = new WordListCriteriaContract
            {
                Key = CriteriaKey.Title,
                Disjunctions = new List<WordCriteriaContract>
                {
                    new WordCriteriaContract
                    {
                        EndsWith = "Romanorum_",
                        StartsWith = "_Gesta"
                    },
                    new WordCriteriaContract
                    {
                        StartsWith = "_Sbírka",
                        Contains = new List<string> {"založených", "na"},
                        EndsWith = "legendách_"
                    }
                }
            };


            var title2 = new WordListCriteriaContract
            {
                Key = CriteriaKey.Title,
                Disjunctions = new List<WordCriteriaContract>
                {
                    new WordCriteriaContract
                    {
                        Contains = new List<string> {"Ge%", "%oman"}
                    },
                    new WordCriteriaContract
                    {
                        Contains = new List<string> {"al_žený"} //založených
                    }
                }
            };

            var editor1 = new WordListCriteriaContract
            {
                Key = CriteriaKey.Editor,
                Disjunctions = new List<WordCriteriaContract>
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
                Disjunctions = new List<WordCriteriaContract>
                {
                    new WordCriteriaContract
                    {
                        Contains = new List<string> {"Kate_ina"}
                    },
                    new WordCriteriaContract
                    {
                        Contains = new List<string> {"Barbora"}
                    }
                }
            };

            var fulltext1 = new WordListCriteriaContract
            {
                Key = CriteriaKey.Fulltext,
                Disjunctions = new List<WordCriteriaContract>
                {
                    //new WordCriteriaContract
                    //{
                    //    Contains = new List<string>{"T%s v Ř_mě"} // Titus v Římě    ---- Will this match if it's not single word?
                    //},
                    new WordCriteriaContract
                    {
                        Contains = new List<string> {"p%st_n%"} // pústeník
                    }
                }
            };

            var fulltext2 = new WordListCriteriaContract
            {
                Key = CriteriaKey.Fulltext,
                Disjunctions = new List<WordCriteriaContract>
                {
                    new WordCriteriaContract
                    {
                        StartsWith = "Ti",
                        Contains = new List<string> {"tu", "tus"},
                        EndsWith = "s"
                    }
                }
            };

            var sentence1 = new WordListCriteriaContract
            {
                Key = CriteriaKey.Sentence,
                Disjunctions = new List<WordCriteriaContract>
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
                Disjunctions = new List<WordCriteriaContract>
                {
                    new WordCriteriaContract
                    {
                        Contains = new List<string> {"prvorození"}
                    }
                }
            };

            var heading1 = new WordListCriteriaContract
            {
                Key = CriteriaKey.Heading,
                Disjunctions = new List<WordCriteriaContract>
                {
                    new WordCriteriaContract
                    {
                        Contains = new List<string> {"lenosti"}
                    }
                }
            };

            var heading2 = new WordListCriteriaContract
            {
                Key = CriteriaKey.Heading,
                Disjunctions = new List<WordCriteriaContract>
                {
                    new WordCriteriaContract
                    {
                        Contains = new List<string> {"synóv"}
                    }
                }
            };

            var tokens = new TokenDistanceListCriteriaContract
            {
                Key = CriteriaKey.TokenDistance,
                Disjunctions = new List<TokenDistanceCriteriaContract>
                {
                    new TokenDistanceCriteriaContract
                    {
                        Distance = 3,
                        First = new WordCriteriaContract
                        {
                            StartsWith = "Dor",
                            Contains = new List<string> {"ote", "us"},
                            EndsWith = "%"
                        },
                        Second = new WordCriteriaContract
                        {
                            Contains = new List<string> {"zákon"}
                        }
                    },
                    new TokenDistanceCriteriaContract
                    {
                        Distance = 10,
                        First = new WordCriteriaContract
                        {
                            StartsWith = "Gor",
                            Contains = new List<string> {"gon", "iu"},
                            EndsWith = "%"
                        },
                        Second = new WordCriteriaContract
                        {
                            Contains = new List<string> {"bíše"}
                        }
                    }
                }
            };

            var resultCriteria = new ResultCriteriaContract
            {
                Count = 25,
                Direction = ListSortDirection.Ascending,
                Sorting = SortEnum.Title,
                Start = 26,
                HitSettingsContract = new HitSettingsContract
                {
                    ContextLength = 70,
                    Count = 3,
                    Start = 1
                }
            };

            var wordListCriteriaContracts = new List<SearchCriteriaContract>
            {
                title1,
                title2,
                editor1,
                editor2,
                fulltext1,
                fulltext2,
                sentence1,
                sentence2,
                heading1,
                heading2,
                tokens,
                resultCriteria
            };


            var resultsCount = m_serviceClient.GetCorpusSearchResultsCount(wordListCriteriaContracts);
            var results = m_serviceClient.GetCorpusSearchResults(wordListCriteriaContracts);
            return Json(results, JsonRequestBehavior.AllowGet);
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