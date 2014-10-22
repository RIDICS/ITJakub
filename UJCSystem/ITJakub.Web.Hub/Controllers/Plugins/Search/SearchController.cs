using System.Collections.Generic;
using System.Web.Mvc;

namespace ITJakub.Web.Hub.Controllers.Plugins.Search
{
    public class SearchController : Controller
    {
        // GET: Bibliographies
        public ActionResult Books(IEnumerable<string> bookIds)
        {
            List<BookInfo> listBooks = new List<BookInfo>();

            foreach (string bookId in bookIds)
            {
                var bookType = "Edition";
                if (int.Parse(bookId)%2 == 0)
                {
                    bookType = "Dictionary";
                }
                if (int.Parse(bookId) % 3 == 0)
                {
                    bookType = "OldCzechTextBank";
                }
                listBooks.Add(new BookInfo{BookId = bookId,BookType = bookType,Body = "here is body",Editor = "Alenka Cerna",Copyright = "alenka",LastEditation = "25.6.1989",LiteraryGenre = "proza",LiteraryType = "type",Name = "Rukopis kunhuta",Pattern = "Broucci",RelicAbbreviation = "a",SourceAbbreviation = "", Pages = new List<Page>(){new Page(){Start = "0", End = "4"}, new Page(){Start = "15", End = "45"}, new Page(){Start = "200"}}}); 
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
                    Body = "here is body",
                    Editor = "Alenka Cerna",
                    Copyright = "alenka",
                    LastEditation = "25.6.1989",
                    LiteraryGenre = "proza",
                    LiteraryType = "type",
                    Name = "Rukopis kunhuta",
                    Pattern = "Broucci",
                    RelicAbbreviation = "a",
                    SourceAbbreviation = ""
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
        public string Body { get; set; }
        public string Editor { get; set; }
        public string Pattern { get; set; }
        public string SourceAbbreviation { get; set; }
        public string RelicAbbreviation { get; set; }
        public string LiteraryType { get; set; }
        public string LiteraryGenre { get; set; }
        public string LastEditation { get; set; }
        public string Copyright { get; set; }

        public List<Page> Pages { get; set; }
    }

    public class Page
    {
        public string Start { get; set; }
        public string End { get; set; }
    }
}