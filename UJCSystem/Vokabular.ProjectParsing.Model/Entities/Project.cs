using System.Collections.Generic;

namespace Vokabular.ProjectParsing.Model.Entities
{
    public class Project
    {
        public string Name { get; set; }

        public IList<Author> Authors { get; }

        public IList<string> Keywords { get; }

        public IList<string> LiteraryGenres { get; }

        public IList<string> LiteraryKinds { get; }

        public IList<string> LiteraryOriginals { get; set; }

        public List<ResponsibleData> Responsibles { get; set; }

        public IList<CategoryData> AllCategoriesHierarchy { get; }

        public ProjectMetadata ProjectMetadata { get; }

        public Project()
        {
            Authors = new List<Author>();
            Keywords = new List<string>();
            LiteraryGenres = new List<string>();
            LiteraryKinds = new List<string>();
            LiteraryOriginals = new List<string>();
            AllCategoriesHierarchy = new List<CategoryData>();
            ProjectMetadata = new ProjectMetadata();
        }
    }
}
