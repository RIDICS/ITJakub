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
            Categorie self = m_allCategories.FirstOrDefault(x => x.Id == id) as Categorie;

            if (self != null) 
                return self.Subitems;
            return null;
        }

        private void LoadTaxonomy()
        {
            var dict = new Categorie {Id = "taxonomy-dictionary", Name = "slovník"};
            dict.Subitems.Add( new Categorie { Id = "taxonomy-dictionary-contemporary", Name = "soudobý" });
            dict.Subitems.Add(new Categorie {Id = "taxonomy-dictionary-historical", Name = "dobový"});
            m_rootCategories.Add(dict);

            var histText = new Categorie { Id = "taxonomy-historical_text", Name = "historický text" };
            histText.Subitems.Add(new Categorie { Id = "taxonomy-historical_text-old_czech", Name = "staročeský" });
            histText.Subitems.Add(new Categorie {Id = "taxonomy-historical_text-medieval_czech", Name = "středněčeský"});

            m_rootCategories.Add(histText);

            var scholarText = new Categorie { Id = "taxonomy-scholary_text", Name = "odborný text" };
            m_rootCategories.Add(scholarText);

            var grammar = new Categorie { Id = "taxonomy-digitized-grammar", Name = "digitalizovaná mluvnice" };
            m_rootCategories.Add(grammar);

            var cards = new Categorie { Id = "taxonomy-card-index", Name = "lístková kartotéka" };
            m_rootCategories.Add(cards);

            m_allCategories.AddRange(m_rootCategories);
            foreach (Categorie cat in m_rootCategories.OfType<Categorie>())
            {
                m_allCategories.AddRange(cat.Subitems);
            }
        }


        private void LoadBooks()
        {
            const string book1Id = "{2A100BE0-D058-486C-8E27-63801CDFDA22}";
            var bookTitle = m_searchClient.GetTitleById(book1Id);

            Categorie category = m_allCategories.FirstOrDefault(x => x.Id == "taxonomy-historical_text-medieval_czech") as Categorie;
            if (category != null)
                category.Subitems.Add(new Book { Id = book1Id, Name = bookTitle });



            string book2Id = "{8C922B93-1185-4B16-BCFC-B8F7A05F1082}";
            bookTitle = m_searchClient.GetTitleById(book2Id);

            category = m_allCategories.FirstOrDefault(x => x.Id == "taxonomy-historical_text-medieval_czech") as Categorie;
            if (category != null)
                category.Subitems.Add(new Book { Id = book2Id, Name = bookTitle });


            string book3Id = "{E776D714-0D0A-475A-962F-FC9F8CCAC846}";
            bookTitle = m_searchClient.GetTitleById(book3Id);

            category = m_allCategories.FirstOrDefault(x => x.Id == "taxonomy-historical_text-old_czech") as Categorie;
            if (category != null)
                category.Subitems.Add(new Book { Id = book3Id, Name = bookTitle });
        }

    }    
}
