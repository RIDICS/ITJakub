using System.Collections.Generic;

namespace Vokabular.ProjectParsing.Model.Entities
{
    public class Project
    {
        public string Name { get; set; }

        public IList<Author> Authors { get; }

        public IList<Keyword> Keywords { get; }

        public MetadataResource MetadataResource { get; set; }

        public Project()
        {
            Authors = new List<Author>();
            Keywords = new List<Keyword>();
            MetadataResource = new MetadataResource();
        }
    }
}
