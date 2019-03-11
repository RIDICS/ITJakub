using System.Collections.Generic;

namespace Vokabular.ProjectParsing.Model.Entities
{
    public class Project
    {
        public string Name { get; set; }

        public IList<Author> Authors { get; }

        public IList<Keyword> Keywords { get; }

        public IList<string> Genres { get; }

        public MetadataResource MetadataResource { get; set; }

        public string EditionNote { get; set; }

        public Project()
        {
            Authors = new List<Author>();
            Keywords = new List<Keyword>();
            Genres = new List<string>();
            MetadataResource = new MetadataResource();
        }
    }
}
