using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Web.Mvc;
using AutoMapper;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Searching.Criteria;
using ITJakub.Shared.Contracts.Searching.Results;
using ITJakub.Web.Hub.Areas.Editions.Models;
using ITJakub.Web.Hub.Converters;
using ITJakub.Web.Hub.Models.Plugins.RegExSearch;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;

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
            IEnumerable<SearchResultContract> listBooks = term.IsNullOrWhiteSpace()
                ? m_serviceClient.GetBooksByBookType(BookTypeEnumContract.Edition)
                : m_serviceClient.SearchBooksWithBookType(term, BookTypeEnumContract.Edition);

            foreach (var list in listBooks)
            {
                list.CreateTimeString = list.CreateTime.ToString();
            }
            return Json(new {books = listBooks}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Listing(string bookId)
        {
            var book = m_serviceClient.GetBookInfo(bookId);
            return
                View(new BookListingModel
                {
                    BookXmlId = book.BookXmlId,
                    BookTitle = book.Title,
                    BookPages = book.BookPages
                });
        }

        public FileResult GetBookImage(string bookId, int position)
        {
            var imageDataStream = m_serviceClient.GetBookPageImage(new BookPageImageContract
            {
                BookXmlId = bookId,
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

        public ActionResult GetEditionsWithCategories()
        {
            var editionsWithCategories = m_serviceClient.GetBooksWithCategoriesByBookType(BookTypeEnumContract.Edition);
            return Json(editionsWithCategories, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AdvancedSearch(string json, short? sortingEnum, bool? sortAsc, IList<long> selectedBookIds, IList<int> selectedCategoryIds)
        {
            var deserialized = JsonConvert.DeserializeObject<IList<ConditionCriteriaDescriptionBase>>(json,
                new ConditionCriteriaDescriptionConverter());
            var listSearchCriteriaContracts = Mapper.Map<IList<SearchCriteriaContract>>(deserialized);

            listSearchCriteriaContracts.Add(new ResultCriteriaContract
            {
                Sorting = (SortEnum) sortingEnum,
                Direction = ListSortDirection.Ascending,
                HitSettingsContract = new HitSettingsContract
                {
                    ContextLength = 70,
                    Count = 3,
                    Start = 1
                }
            });

            listSearchCriteriaContracts.Add(new SelectedCategoryCriteriaContract
            {
                SelectedBookIds = new List<long> {1, 2},
                SelectedCategoryIds = new List<int> {1, 2} //TODO
            });


            var results = m_serviceClient.SearchByCriteria(listSearchCriteriaContracts);
            return Json(new {results});
        }

        public ActionResult AdvancedSearchPaged(string json, int start, int count, short? sortingEnum, bool? sortAsc)
        {
            var deserialized = JsonConvert.DeserializeObject<IList<ConditionCriteriaDescriptionBase>>(json,
                new ConditionCriteriaDescriptionConverter());
            var listSearchCriteriaContracts = Mapper.Map<IList<SearchCriteriaContract>>(deserialized);

            listSearchCriteriaContracts.Add(new ResultCriteriaContract
            {
                Start = start,
                Count = count,
                Sorting = (SortEnum) sortingEnum,
                Direction = ListSortDirection.Ascending,
                HitSettingsContract = new HitSettingsContract
                {
                    ContextLength = 70,
                    Count = 3,
                    Start = 1
                }
            });

            listSearchCriteriaContracts.Add(new SelectedCategoryCriteriaContract
            {
                SelectedBookIds = new List<long> {1, 2},
                SelectedCategoryIds = new List<int> {1, 2} //TODO
            });


            var results = m_serviceClient.SearchByCriteria(listSearchCriteriaContracts);
            return Json(new {results});
        }

        public ActionResult TextSearch(string text, int? pageNumber)
        {
            //m_serviceClient.SearchByCriteria(listSearchCriteriaContracts); //TODO implement
            return Json(new {results = "TODO"});
        }

        public ActionResult SearchCriteriaMocked()
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

            //Mockup of search result 
            var createTime = DateTime.Today;
            var resultSearchCrit = new SearchResultContract
            {
                Authors =
                    new List<AuthorContract>
                    {
                        new AuthorContract {Name = "autor1"},
                        new AuthorContract {Name = "autor2"}
                    },
                BookXmlId = "xmlIdKnihy",
                VersionXmlId = "xmlVerzeKnihy",
                BookType = BookTypeEnumContract.Edition,
                Copyright = "text copyrightu",
                CreateTime = createTime,
                CreateTimeString = createTime.ToString(CultureInfo.InvariantCulture),
                PublishDate = "Publikovano roku 1989",
                Editors =
                    new List<EditorContract>
                    {
                        new EditorContract {Text = "editor1"},
                        new EditorContract {Text = "editor2"}
                    },
                Keywords = new List<string> {"pes", "kocka"},
                Manuscripts =
                    new List<ManuscriptContract>
                    {
                        new ManuscriptContract
                        {
                            Title = "Titul",
                            Country = "Zeme",
                            Idno = "Idno",
                            OriginDate = "Datum",
                            Repository = "repositar",
                            Settlement = "Osada"
                        }
                    },
                PageCount = 426,
                PublishPlace = "Praha",
                Publisher = new PublisherContract {Email = "a@a.cz", Text = "publikator"},
                Title = "Titul dila",
                SubTitle = "Podtitul dila",
                TotalHitCount = 15,
                Results = new List<PageResultContext>
                {
                    new PageResultContext
                    {
                        ContextStructure = new KwicStructure
                        {
                            Before = "...zacalo to pred malym ",
                            Match = "psem",
                            After = ", ktery nemel rad kocky..."
                        },
                        PageName = "2r",
                        PageXmlId = "div1.pb2"
                    },
                    new PageResultContext
                    {
                        ContextStructure = new KwicStructure
                        {
                            Before = "...zacalo to po malem ",
                            Match = "psu",
                            After = ", ktery nikdy nebyl venku..."
                        },
                        PageName = "145r",
                        PageXmlId = "div145.pb55"
                    },
                    new PageResultContext
                    {
                        ContextStructure = new KwicStructure
                        {
                            Before = "...skoncilo to ",
                            Match = "psem",
                            After = ", ktery byl stasten..."
                        },
                        PageName = "210v",
                        PageXmlId = "div5.pb45"
                    }
                }
            };

            var resultSearchCrit2 = new SearchResultContract
            {
                Authors =
                    new List<AuthorContract>
                    {
                        new AuthorContract {Name = "autor1"},
                        new AuthorContract {Name = "autor2"}
                    },
                BookXmlId = "xmlIdKnihy2",
                VersionXmlId = "xmlVerzeKnihy2",
                BookType = BookTypeEnumContract.Edition,
                Copyright = "text copyrightu",
                CreateTime = createTime,
                CreateTimeString = createTime.ToString(CultureInfo.InvariantCulture),
                PublishDate = "Publikovano roku 1989",
                Editors =
                    new List<EditorContract>
                    {
                        new EditorContract {Text = "editor1"},
                        new EditorContract {Text = "editor2"}
                    },
                Keywords = new List<string> {"pes", "kocka"},
                Manuscripts =
                    new List<ManuscriptContract>
                    {
                        new ManuscriptContract
                        {
                            Title = "Titul",
                            Country = "Zeme",
                            Idno = "Idno",
                            OriginDate = "Datum",
                            Repository = "repositar",
                            Settlement = "Osada"
                        }
                    },
                PageCount = 426,
                PublishPlace = "Praha",
                Publisher = new PublisherContract {Email = "a@a.cz", Text = "publikator"},
                Title = "Titul dila",
                SubTitle = "Podtitul dila",
                TotalHitCount = 15,
                Results = new List<PageResultContext>
                {
                    new PageResultContext
                    {
                        ContextStructure = new KwicStructure
                        {
                            Before = "...zacalo to pred malym ",
                            Match = "psem",
                            After = ", ktery nemel rad kocky..."
                        },
                        PageName = "2r",
                        PageXmlId = "div1.pb2"
                    },
                    new PageResultContext
                    {
                        ContextStructure = new KwicStructure
                        {
                            Before = "...zacalo to po malem ",
                            Match = "psu",
                            After = ", ktery nikdy nebyl venku..."
                        },
                        PageName = "145r",
                        PageXmlId = "div145.pb55"
                    },
                    new PageResultContext
                    {
                        ContextStructure = new KwicStructure
                        {
                            Before = "...skoncilo to ",
                            Match = "psem",
                            After = ", ktery byl stasten..."
                        },
                        PageName = "210v",
                        PageXmlId = "div5.pb45"
                    }
                }
            };

            //string bResult = string.Empty;

            //using (Stream stream = new MemoryStream())
            //{
            //    //Serialize the Record object to a memory stream using DataContractSerializer. 
            //    DataContractSerializer serializer = new DataContractSerializer(typeof(List<SearchResultContract>));
            //    serializer.WriteObject(stream, new List<SearchResultContract> { resultSearchCrit, resultSearchCrit2 });
            //    stream.Position = 0;
            //    string result = new StreamReader(stream).ReadToEnd();
            //    bResult = result;
            //}

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
                tokens
            };
            m_serviceClient.SearchByCriteria(wordListCriteriaContracts);
            return Json(new {}, JsonRequestBehavior.AllowGet);
        }
    }
}