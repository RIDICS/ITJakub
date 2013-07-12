using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ITJakub.Contracts.Categories;
using log4net;

namespace ITJakub.Core.Database
{
    public class ReleationDatabaseMock
    {
        private readonly List<SelectionBase> m_allCategories = new List<SelectionBase>();
        private readonly List<SelectionBase> m_rootCategories = new List<SelectionBase>();
        private readonly List<Book> m_allBooks = new List<Book>();
        private readonly SearchServiceClient m_searchClient;
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ReleationDatabaseMock(SearchServiceClient searchClient)
        {
            m_searchClient = searchClient;
            LoadTaxonomy();
            LoadBooks();

            AdjustCategoryChildrenExistence();
        }

        private void AdjustCategoryChildrenExistence()
        {
            foreach (Category category in m_allCategories.Select(item => item as Category).Where(category => category != null))
            {
                if (category.Subitems != null && category.Subitems.Count != 0)
                    category.HasChildren = true;
            }
        }


        public List<SelectionBase> GetRootCategories()
        {
            return m_rootCategories;
        }

        public List<SelectionBase> GetChildren(string id)
        {
            var self = m_allCategories.FirstOrDefault(x => x.Id == id) as Category;

            if (self != null)
                return self.Subitems;
            return null;
        }

        private void LoadTaxonomy()
        {
            var dict = new Category {Id = "taxonomy-dictionary", Name = "slovník", TextValue = "Slovníky"};
            dict.Subitems.Add(new Category {Id = "taxonomy-dictionary-contemporary", Name = "soudobý", TextValue = "Soudobé", Parrent = dict});
            dict.Subitems.Add(new Category {Id = "taxonomy-dictionary-historical", Name = "dobový", TextValue = "Dobové", Parrent = dict});
            m_rootCategories.Add(dict);

            var histText = new Category {Id = "taxonomy-historical_text", Name = "historický text", TextValue = "Historické texty"};
            histText.Subitems.Add(new Category {Id = "taxonomy-historical_text-old_czech", Name = "staročeský", TextValue = "Staročeské", Parrent = histText});
            histText.Subitems.Add(new Category
                {
                    Id = "taxonomy-historical_text-medieval_czech",
                    Name = "středněčeský",
                    TextValue = "Středněčeské",
                    Parrent = histText,
                    ShowType = CategoryShowType.SelectionBox
                });

            m_rootCategories.Add(histText);

            var scholarText = new Category {Id = "taxonomy-scholary_text", Name = "odborný text", TextValue = "Odborné texty"};
            m_rootCategories.Add(scholarText);

            var grammar = new Category {Id = "taxonomy-digitized-grammar", Name = "digitalizovaná mluvnice", TextValue = "Digitalizované mluvnice"};
            m_rootCategories.Add(grammar);

            var cards = new Category {Id = "taxonomy-card-index", Name = "lístková kartotéka", TextValue = "Lístková kartotéka"};
            m_rootCategories.Add(cards);

            m_allCategories.AddRange(m_rootCategories);
            foreach (Category cat in m_rootCategories.OfType<Category>())
            {
                m_allCategories.AddRange(cat.Subitems);
            }
        }

        private void AddBook(string bookId, string categoryId)
        {
            if (m_log.IsInfoEnabled)
                m_log.InfoFormat("Adding book with id: {0}", bookId);

            string bookTitle = m_searchClient.GetTitleById(bookId);

            var category = m_allCategories.FirstOrDefault(x => x.Id == categoryId) as Category;
            if (category != null)
            {
                string catClassification = GetCategoryTextValue(category);
                var book = new Book {Id = bookId, Name = bookTitle};
                book.TextCategoriesClassification.Add(catClassification);
                category.Subitems.Add(book);
                m_allBooks.Add(book);
            }
        }

        private string GetCategoryTextValue(Category category)
        {
            List<Category> levels = new List<Category>();
            Category level = category;
            while (level != null)
            {
                levels.Add(level);
                level = level.Parrent;
            }

            levels.Reverse();
            return string.Join(" – ", levels.Select(x => x.TextValue));
        }

        private void LoadBooks()
        {
            AddBook("2A100BE0-D058-486C-8E27-63801CDFDA22", "taxonomy-historical_text-medieval_czech");
            AddBook("8C922B93-1185-4B16-BCFC-B8F7A05F1082", "taxonomy-historical_text-medieval_czech");
            AddBook("E776D714-0D0A-475A-962F-FC9F8CCAC846", "taxonomy-historical_text-old_czech");
            AddBook("8688926F-9106-4A70-9440-673779415D07", "taxonomy-historical_text-old_czech");
            AddBook("E494DBC5-F3C4-4841-B4D3-C52FE99839EB", "taxonomy-historical_text-old_czech");
            AddBook("FA10177B-25E6-4BB6-B061-0DB988AD3840", "taxonomy-historical_text-old_czech");
            AddBook("D122AA8E-BE32-4EAF-B274-011CBBD2A01B", "taxonomy-historical_text-old_czech");
            AddBook("DB31F937-74B1-45A9-B976-8672FA1DC8C7", "taxonomy-historical_text-old_czech");
            AddBook("125A0032-03B5-40EC-B68D-80473CC5653A", "taxonomy-historical_text-old_czech");
            AddBook("9BE99DC1-C7F1-4F89-882C-4C8C07FE25F5", "taxonomy-historical_text-old_czech");
            AddBook("59C0C5DC-300A-42E4-8BF2-0CB4874E8255", "taxonomy-scholary_text");
            AddBook("1A74599E-3A19-44AA-AB1A-EF54077B81DB", "taxonomy-scholary_text");
            AddBook("CC8D75B1-06B3-4C8E-9B4C-E366FEE76175", "taxonomy-scholary_text");
            AddBook("D7ACD09E-8778-4246-86E7-126CCC98F1FF", "taxonomy-scholary_text");
            AddBook("640F2E22-5443-496B-BCB6-A6184EE43388", "taxonomy-scholary_text");
            AddBook("795E13F1-B52A-402F-B716-D06DFE7A9811", "taxonomy-scholary_text");
            AddBook("719AA5BF-E24D-45C0-A835-FAC774B1FFF8", "taxonomy-scholary_text");
            AddBook("CACB63F9-B6AE-4C9C-9101-584F8100BDB4", "taxonomy-scholary_text");
            AddBook("66C9C773-7542-4820-A4F9-71C180CBFDEB", "taxonomy-scholary_text");
            AddBook("DBB04F82-912D-4252-ACB2-FAF43D3A8E2C", "taxonomy-dictionary-historical");
            AddBook("4E5DB418-B49B-4AC0-AE9F-78A53E9BE4FE", "taxonomy-dictionary-contemporary");
            AddBook("6810F0EC-989E-42F1-A2E3-2D22B5E67EC7", "taxonomy-dictionary-contemporary");
        }

        public List<string> GetBookIdsByCategories(IEnumerable<string> categorieIds)
        {
            var result = new List<string>();
            foreach (string categorieId in categorieIds)
            {
                var category = m_allCategories.FirstOrDefault(x => x.Id == categorieId) as Category;

                if (category != null)
                {
                    foreach (SelectionBase subitem in category.Subitems)
                    {
                        if (subitem is Book && !result.Contains(subitem.Id))
                            result.Add(subitem.Id);
                    }
                }
            }
            return result;
        }

        public List<SelectionBase> GetSelectedTreePart(List<string> categorieIds, List<string> booksIds)
        {
            return m_rootCategories;
        }

        public Book GetBookById(string bookId)
        {
            return m_allBooks.FirstOrDefault(x => x.Id == bookId);
        }

        public List<string> GetCategoryByBookId(string id)
        {
            var book = m_allBooks.FirstOrDefault(x => x.Id == id);
            if (book != null)
            {
                return book.TextCategoriesClassification;
            }
            return new List<string>();
        }
    }
}