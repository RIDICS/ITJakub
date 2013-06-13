using System.Collections.Generic;
using System.Linq;
using ITJakub.Contracts.Categories;

namespace ITJakub.Core.Database
{
    public class ReleationDatabaseMock
    {
        private readonly List<SelectionBase> m_allCategories = new List<SelectionBase>();
        private readonly List<SelectionBase> m_rootCategories = new List<SelectionBase>();

        private readonly SearchServiceClient m_searchClient;

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
            var dict = new Category {Id = "taxonomy-dictionary", Name = "slovník"};
            dict.Subitems.Add(new Category {Id = "taxonomy-dictionary-contemporary", Name = "soudobý"});
            dict.Subitems.Add(new Category {Id = "taxonomy-dictionary-historical", Name = "dobový"});
            m_rootCategories.Add(dict);

            var histText = new Category {Id = "taxonomy-historical_text", Name = "historický text"};
            histText.Subitems.Add(new Category {Id = "taxonomy-historical_text-old_czech", Name = "staročeský"});
            histText.Subitems.Add(new Category {Id = "taxonomy-historical_text-medieval_czech", Name = "středněčeský", ShowType = CategoryShowType.SelectionBox});

            m_rootCategories.Add(histText);

            var scholarText = new Category {Id = "taxonomy-scholary_text", Name = "odborný text"};
            m_rootCategories.Add(scholarText);

            var grammar = new Category {Id = "taxonomy-digitized-grammar", Name = "digitalizovaná mluvnice"};
            m_rootCategories.Add(grammar);

            var cards = new Category {Id = "taxonomy-card-index", Name = "lístková kartotéka"};
            m_rootCategories.Add(cards);

            m_allCategories.AddRange(m_rootCategories);
            foreach (Category cat in m_rootCategories.OfType<Category>())
            {
                m_allCategories.AddRange(cat.Subitems);
            }
        }


        private void LoadBooks()
        {
            string book1Id = "59C0C5DC-300A-42E4-8BF2-0CB4874E8255";
            string bookTitle = m_searchClient.GetTitleById(book1Id);

            var category = m_allCategories.FirstOrDefault(x => x.Id == "taxonomy-scholary_text") as Category;
            if (category != null)
                category.Subitems.Add(new Book {Id = book1Id, Name = bookTitle});

            string book2Id = "66C9C773-7542-4820-A4F9-71C180CBFDEB";
            bookTitle = m_searchClient.GetTitleById(book2Id);

            category = m_allCategories.FirstOrDefault(x => x.Id == "taxonomy-historical_text-old_czech") as Category;
            if (category != null)
                category.Subitems.Add(new Book {Id = book2Id, Name = bookTitle});


            string book3Id = "CACB63F9-B6AE-4C9C-9101-584F8100BDB4";
            bookTitle = m_searchClient.GetTitleById(book3Id);

            category = m_allCategories.FirstOrDefault(x => x.Id == "taxonomy-historical_text-old_czech") as Category;
            if (category != null)
                category.Subitems.Add(new Book {Id = book3Id, Name = bookTitle});


            //test book in two categories
            string bookId4 = "1A74599E-3A19-44AA-AB1A-EF54077B81DB";
            bookTitle = m_searchClient.GetTitleById(bookId4);
            var book = new Book {Id = bookId4, Name = bookTitle};
            category = m_allCategories.FirstOrDefault(x => x.Id == "taxonomy-scholary_text") as Category;
            if (category != null)
                category.Subitems.Add(book);
            category = m_allCategories.FirstOrDefault(x => x.Id == "taxonomy-historical_text-medieval_czech") as Category;
            if (category != null)
                category.Subitems.Add(book);


            //slovniky
            string bookId5 = "DBB04F82-912D-4252-ACB2-FAF43D3A8E2C";
            bookTitle = m_searchClient.GetTitleById(bookId5);
            category = m_allCategories.FirstOrDefault(x => x.Id == "taxonomy-dictionary-historical") as Category;
            if (category != null)
                category.Subitems.Add(new Book {Id = bookId5, Name = bookTitle});


            string bookId6 = "4E5DB418-B49B-4AC0-AE9F-78A53E9BE4FE";
            bookTitle = m_searchClient.GetTitleById(bookId6);
            category = m_allCategories.FirstOrDefault(x => x.Id == "taxonomy-dictionary-contemporary") as Category;
            if (category != null)
                category.Subitems.Add(new Book {Id = bookId6, Name = bookTitle});


            string bookId7 = "6810F0EC-989E-42F1-A2E3-2D22B5E67EC7";
            bookTitle = m_searchClient.GetTitleById(bookId7);
            category = m_allCategories.FirstOrDefault(x => x.Id == "taxonomy-dictionary-contemporary") as Category;
            if (category != null)
                category.Subitems.Add(new Book {Id = bookId7, Name = bookTitle});



            //medival czech books
            string bookId8 = "2A100BE0-D058-486C-8E27-63801CDFDA22";
            bookTitle = m_searchClient.GetTitleById(bookId8);
            category = m_allCategories.FirstOrDefault(x => x.Id == "taxonomy-historical_text-medieval_czech") as Category;
            if (category != null)
                category.Subitems.Add(new Book { Id = bookId8, Name = bookTitle });

            string bookId9 = "8C922B93-1185-4B16-BCFC-B8F7A05F1082";
            bookTitle = m_searchClient.GetTitleById(bookId9);
            category = m_allCategories.FirstOrDefault(x => x.Id == "taxonomy-historical_text-medieval_czech") as Category;
            if (category != null)
                category.Subitems.Add(new Book { Id = bookId9, Name = bookTitle });
        }

        public List<string> GetBookIdsByCategories(List<string> categorieIds)
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
    }
}