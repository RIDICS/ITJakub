using System.Collections.Generic;

namespace Vokabular.ProjectParsing.Model.Entities
{
    public class Project
    {
        public string Name { get; set; }

        public HashSet<Author> Authors { get; set; }

        public IList<string> Keywords { get; set; }

        public IList<string> LiteraryGenres { get; set; }

        public IList<string> LiteraryOriginals { get; set; }

        public ProjectMetadata ProjectMetadata { get; set; }

        public Project()
        {
            Authors = new HashSet<Author>();
            Keywords = new List<string>();
            LiteraryGenres = new List<string>();
            LiteraryOriginals = new List<string>();
            ProjectMetadata = new ProjectMetadata();
        }
    }
}
