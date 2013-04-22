using System.Collections.Generic;
using Ujc.Naki.MVCWebLayer.Services.DTOs;
using Ujc.Naki.MVCWebLayer.Services.Enums;

namespace Ujc.Naki.MVCWebLayer.Services.Mocks
{
    public class BookCategoryService
    {
        private static Dictionary<string, Book> Books = new Dictionary<string,Book>() 
        {
            { "dictionary", new Book("Slovníky", BookCategory.Dictionary, true) },
            { "historic", new Book("Historické texty", BookCategory.Historic, true) },
            { "technical", new Book("Odborné texty", BookCategory.Technical, true) },
            { "grammar", new Book("Digitalizované mluvnice", BookCategory.Digitalized, true) },
            { "lists", new Book("Lístkové kartotéky", BookCategory.Listed, true) },

            { "current-dictionary", new Book("Soudobé slovníky", BookCategory.Dictionary, false) },
            { "old-dictionary", new Book("Dobové slovníky", BookCategory.Dictionary, false) },
            { "digitalized-dictionary", new Book("Digitalizované slovníky", BookCategory.Dictionary, false) },

            { "mss", new Book("Malý staročeský slovník (MSS)", BookCategory.Dictionary, false) },
            { "mss2", new Book("Malý staročeský slovník (MSS)", BookCategory.Dictionary, false) },
            { "essc", new Book("Elektronický slovník staré češtiny (ESSČ)", BookCategory.Dictionary, false) },
            { "essc2", new Book("Elektronický slovník staré češtiny (ESSČ)", BookCategory.Dictionary, false) },
            { "gbslov", new Book("Jan Gebauer: Slovník staročeský", BookCategory.Dictionary, false) },
            { "gbslov2", new Book("Jan Gebauer: Slovník staročeský", BookCategory.Dictionary, false) },
            { "stcs", new Book("Staročeský slovník (sešit 1-26) (StčS)", BookCategory.Dictionary, false) },
            { "stcs2", new Book("Staročeský slovník (sešit 1-26) (StčS)", BookCategory.Dictionary, false) },
            { "simekslov", new Book("F.Šimek, Slovníček staré češtiny (ŠimekSlov)", BookCategory.Dictionary, false) },
            { "simekslov2", new Book("F.Šimek, Slovníček staré češtiny (ŠimekSlov)", BookCategory.Dictionary, false) },

            { "staroceske", new Book("Staročeské", BookCategory.Historic, false) },
            { "stredneceske", new Book("Středně české", BookCategory.Historic, false) },
            { "slovo", new Book("Slovo do světa", BookCategory.Historic, false) },
            { "aa", new Book("Alexandreida: Zlomek jindrichohradecky", BookCategory.Historic, false) },
        };

        public Dictionary<BookCategory, List<string>> GetSearchedBooks(string[] selected)
        {
            var books = new List<Book>();
            var result = new Dictionary<BookCategory, List<string>>();

            if (selected == null)
            {
                return null;
            }

            foreach (var book in selected)
            {
                Book bookObject;
                if (Books.TryGetValue(book, out bookObject))
                {
                    books.Add(bookObject);
                }
            }

            foreach (var book in books)
            {
                List<string> bookNames;
                if (result.TryGetValue(book.Category, out bookNames))
                {
                    if (!book.IsRoot)
                    {
                        bookNames.Add(book.Name);
                    }
                }
                else
                {
                    if (book.IsRoot)
                    {
                        result.Add(book.Category, new List<string>());
                    }
                    else
                    {
                        bookNames = new List<string>();
                        bookNames.Add(book.Name);
                        result.Add(book.Category, bookNames);
                    }
                }
            }

            return result;
        }
    }
}