using System.Collections.Generic;
using System.Linq;
using ITJakub.Contracts.Categories;

namespace ITJakub.Core.Database
{
    public class ReleationDatabaseMock
    {
        private readonly List<SelectionBase> m_rootCategories = new List<SelectionBase>();
        private readonly List<SelectionBase>  m_allCategories = new List<SelectionBase>();

        private readonly SearchServiceClient m_searchClient;

        public ReleationDatabaseMock(SearchServiceClient searchClient)
        {
            m_searchClient = searchClient;
            LoadTaxonomy();
            LoadBooks();
        }



        public List<SelectionBase> GetRootCategories()
        {
            return m_rootCategories;
        }

        public List<SelectionBase> GetChildren(string id)
        {
            Category self = m_allCategories.FirstOrDefault(x => x.Id == id) as Category;

            if (self != null) 
                return self.Subitems;
            return null;
        }

        private void LoadTaxonomy()
        {
            var dict = new Category {Id = "taxonomy-dictionary", Name = "slovník"};
            dict.Subitems.Add( new Category { Id = "taxonomy-dictionary-contemporary", Name = "soudobý" });
            dict.Subitems.Add(new Category {Id = "taxonomy-dictionary-historical", Name = "dobový"});
            m_rootCategories.Add(dict);

            var histText = new Category { Id = "taxonomy-historical_text", Name = "historický text" };
            histText.Subitems.Add(new Category { Id = "taxonomy-historical_text-old_czech", Name = "staročeský" });
            histText.Subitems.Add(new Category {Id = "taxonomy-historical_text-medieval_czech", Name = "středněčeský"});

            m_rootCategories.Add(histText);

            var scholarText = new Category { Id = "taxonomy-scholary_text", Name = "odborný text" };
            m_rootCategories.Add(scholarText);

            var grammar = new Category { Id = "taxonomy-digitized-grammar", Name = "digitalizovaná mluvnice" };
            m_rootCategories.Add(grammar);

            var cards = new Category { Id = "taxonomy-card-index", Name = "lístková kartotéka" };
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
            var bookTitle = m_searchClient.GetTitleById(book1Id);

            Category category = m_allCategories.FirstOrDefault(x => x.Id == "taxonomy-historical_text-medieval_czech") as Category;
            if (category != null)
                category.Subitems.Add(new Book { Id = book1Id, Name = bookTitle });

            string book2Id = "66C9C773-7542-4820-A4F9-71C180CBFDEB";
            bookTitle = m_searchClient.GetTitleById(book2Id);

            category = m_allCategories.FirstOrDefault(x => x.Id == "taxonomy-historical_text-old_czech") as Category;
            if (category != null)
                category.Subitems.Add(new Book { Id = book2Id, Name = bookTitle });


            string book3Id = "CACB63F9-B6AE-4C9C-9101-584F8100BDB4";
            bookTitle = m_searchClient.GetTitleById(book3Id);

            category = m_allCategories.FirstOrDefault(x => x.Id == "taxonomy-historical_text-old_czech") as Category;
            if (category != null)
                category.Subitems.Add(new Book { Id = book3Id, Name = bookTitle });


            string bookId4 = "1A74599E-3A19-44AA-AB1A-EF54077B81DB";
            bookTitle = m_searchClient.GetTitleById(bookId4);

            category = m_allCategories.FirstOrDefault(x => x.Id == "taxonomy-historical_text-old_czech") as Category;
            if (category != null)
                category.Subitems.Add(new Book { Id = bookId4, Name = bookTitle });

        }

        public List<string> GetBookIdsByCategories(List<string> categorieIds)
        {
            List<string> result = new List<string>();
            foreach (var categorieId in categorieIds)
            {
                Category category = m_allCategories.FirstOrDefault(x => x.Id == categorieId) as Category;

                if (category != null)
                {
                    foreach (var subitem in category.Subitems)
                    {
                        if(subitem is Book && !result.Contains(subitem.Id))
                            result.Add(subitem.Id);
                    }
                }

            }
            return result;
        }
    }    
}
