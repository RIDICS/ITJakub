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

            foreach (string bookId in bookIds)
            {
                string bookType = "Edition";
                if (int.Parse(bookId)%2 == 0)
                {
                    bookType = "Dictionary";
                }
                if (int.Parse(bookId)%3 == 0)
                {
                    bookType = "OldCzechTextBank";
                }
                if (int.Parse(bookId) % 4 == 0)
                {
                    bookType = "CardFile";
                }
                listBooks.Add(new BookInfo
                {
                    BookId = bookId,
                    BookType = bookType,
                    Editor = "Alenka Cerna",
                    Copyright = "Černá, Alena M.,2013, oddělení vývoje jazyka UJC AV CR, v.v.i., 2013",
                    LastEditation = "25.6.1989",
                    LiteraryGenre = "proza",
                    LiteraryType = "type",
                    Name = "Rukopis kunhuta",
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
                    Archive = new Archive() { Name = "Narodni knihovna ceske republiky", City = "Praha", State = "Cesko"},
                    Authors = new List<Author>() { new Author() { FirstName = "Josef", LastName = "Novak" }, new Author() { FirstName = "Jaroslav", LastName = "Tuhik" } },
                    Description = "Elementa latinae, boemicae ac germanicae linugae",
                    Year = 1532,
                    TestValue = 0,
                    TestValue2 = false,
                    TestValue3 = true
                });
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
                    Name = "Rukopis kunhuta",
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
            ;
        }
    }

    public class BookInfo
    {
        public string BookId { get; set; }
        public string BookType { get; set; }
        public string Name { get; set; }
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

        public List<Author> Authors { get; set; }

        public Archive Archive { get; set; }

        public List<Page> Pages { get; set; }
        public int TestValue { get; set; }
        public bool TestValue2 { get; set; }
        public bool TestValue3 { get; set; }
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