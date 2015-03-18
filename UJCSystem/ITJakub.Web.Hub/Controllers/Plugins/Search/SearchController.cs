using System.Collections.Generic;
using System.Web.Mvc;

namespace ITJakub.Web.Hub.Controllers.Plugins.Search
{
    public class SearchController : Controller
    {
        // GET: Bibliographies
        public ActionResult Books(IEnumerable<string> bookIds)
        {
            var listBooks = new List<BookInfo>();
            var bookInfo = new BookInfo()
            {
                BookId = "{FA10177B-25E6-4BB6-B061-0DB988AD3840}",
                BookType = "Edition",
                Editor = "Alena Černá",
                Copyright = "Černá, Alena M.,2013, oddělení vývoje jazyka Ústavu pro jazyk český AV ČR, v.v.i., 2012",
                LastEditation = "",
                LiteraryGenre = "próza",
                LiteraryType = "próza",
                Title = "Chirurgické lékařství",
                Pattern = "ChL",
                RelicAbbreviation = "LékChir",
                SourceAbbreviation = "LékChir",
                Pages =
                    new List<Page>
                        {
                            new Page {Start = "0", End = "4"},
                            new Page {Start = "15", End = "45"}
                        },
                Century = 16,
                Sign = "XVII H 23",
                Archive = new Archive() { Name = "Národní knihovna České republiky", City = "Praha", State = "Česko" },
                Authors =
                    new List<Author>()
                        {
                        },
                Description = "",
                Year = 2012
            };
            listBooks.Add(bookInfo);

            bookInfo = new BookInfo()
            {
                BookId = "DB31F937-74B1-45A9-B976-8672FA1DC8C7",
                BookType = "Edition",
                Editor = "Alena Černá",
                Copyright = "Černá, Alena M.,2013, oddělení vývoje jazyka Ústavu pro jazyk český AV ČR, v.v.i., 2012",
                LastEditation = "",
                LiteraryGenre = "próza",
                LiteraryType = "próza",
                Title = "Knihy lékařské",
                Pattern = "KL",
                RelicAbbreviation = "LékJádro",
                SourceAbbreviation = "LékJádroBrn",
                Pages =
                    new List<Page>
                        {
                            new Page {Start = "0", End = "4"}
                        },
                Century = 16,
                Sign = "Rkp 156",
                Archive = new Archive() { Name = "Muzejní archiv", City = "Brno", State = "Česko" },
                Authors =
                    new List<Author>()
                    {
                    },
                Description = "",
                Year = 2012
            };
            listBooks.Add(bookInfo);
            foreach (string bookId in bookIds)
            {
                string bookType = "Edition";
                bookInfo = new BookInfo
               {
                   BookId = bookId,
                   BookType = bookType,
                   Editor = "Alenka Cerna",
                   Copyright = "Černá, Alena M.,2013, oddělení vývoje jazyka UJC AV CR, v.v.i., 2013",
                   LastEditation = "25.6.1989",
                   LiteraryGenre = "proza",
                   LiteraryType = "type",
                   Title = "Rukopis kunhuta",
                   Pattern = "Broucci",
                   RelicAbbreviation = "relAbr",
                   SourceAbbreviation = "RK",
                   Pages =
                       new List<Page>
                        {
                            new Page {Start = "0", End = "4"},
                            new Page {Start = "15", End = "45"},
                            new Page {Start = "200"}
                        },
                   Century = 13,
                   Sign = "VII G 17 d",
                   Archive = new Archive() { Name = "Narodni knihovna ceske republiky", City = "Praha", State = "Cesko" },
                   Authors =
                       new List<Author>()
                        {
                            new Author() {FirstName = "Josef", LastName = "Novak"},
                            new Author() {FirstName = "Jaroslav", LastName = "Tuhik"}
                        },
                   Description = "Elementa latinae, boemicae ac germanicae linugae",
                   Year = 1532
               };

                int parsedBookId;
                var parsed = int.TryParse(bookId, out parsedBookId);
                if (parsed && parsedBookId % 2 == 0)
                {
                    bookInfo.BookType = "CardFile";

                }
                if (parsed && parsedBookId % 3 == 0)
                {
                    bookInfo.BookType = "OldCzechTextBank";
                    bookInfo.Century = 1;
                }
                if (parsed && parsedBookId % 4 == 0)
                {
                    bookInfo.BookType = "Dictionary";
                    bookInfo.Title = "Slovnicek";
                    bookInfo.Century = 5;
                    bookInfo.Linked = true;

                    if (parsed && parsedBookId % 8 == 0)
                    {
                        bookInfo.Linked = false;
                    }
                }
                if (parsed && parsedBookId % 5 == 0)
                {
                    bookInfo.BookType = "UndefBokkType";
                }
                listBooks.Add(bookInfo);
            }

            return Json(new {books = listBooks}, JsonRequestBehavior.AllowGet);
        }

        // GET: Bibliographies
        public ActionResult BooksWithType(string type)
        {
            var listBooks = new List<BookInfo>();
            for (int i = 0; i < type.Length; i++)
            {
                var bookInfo = new BookInfo
                {
                    BookId = type[i] + i.ToString(),
                    BookType = type,
                    Editor = "Alenka Cerna",
                    Copyright = "Černá, Alena M.,2013, oddělení vývoje jazyka UJC AV CR, v.v.i., 2013",
                    LastEditation = "25.6.1989",
                    LiteraryGenre = "proza",
                    LiteraryType = "type",
                    Title = "Rukopis kunhuta",
                    Pattern = "Broucci",
                    RelicAbbreviation = "relAbr",
                    SourceAbbreviation = "RK",
                    Pages =
                        new List<Page>
                        {
                            new Page {Start = "0", End = "4"},
                            new Page {Start = "15", End = "45"},
                            new Page {Start = "200"}
                        },
                    Century = 13,
                    Sign = "VII G 17 d",
                    Archive = new Archive() { Name = "Narodni knihovna ceske republiky", City = "Praha", State = "Cesko" },
                    Authors = new List<Author>() { new Author() { FirstName = "Josef", LastName = "Novak" }, new Author() { FirstName = "Jaroslav", LastName = "Tuhik" } },
                    Description = "Elementa latinae, boemicae ac germanicae linugae",
                    Year = 1532
                };
                listBooks.Add(bookInfo);
            }
            return Json(new {books = listBooks}, JsonRequestBehavior.AllowGet);
            
        }
    }

    public class BookInfo
    {
        public string BookId { get; set; }
        public string BookType { get; set; }
        public string Title { get; set; }
        public string Editor { get; set; }
        public string Pattern { get; set; }
        public string SourceAbbreviation { get; set; }
        public string RelicAbbreviation { get; set; }
        public string LiteraryType { get; set; }
        public string LiteraryGenre { get; set; }
        public string LastEditation { get; set; }
        public string Copyright { get; set; }
        public int Century { get; set; }
        public string Sign { get; set; }
        public string Description { get; set; }
        public int Year { get; set; }
        public bool Linked { get; set; }

        public List<Author> Authors { get; set; }

        public Archive Archive { get; set; }

        public List<Page> Pages { get; set; }
    }

    public class Page
    {
        public string Start { get; set; }
        public string End { get; set; }
    }

    public class Author
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class Archive
    {
        public string Name { get; set; }
        public string City { get; set; }
        public string State { get; set; }
    }
}