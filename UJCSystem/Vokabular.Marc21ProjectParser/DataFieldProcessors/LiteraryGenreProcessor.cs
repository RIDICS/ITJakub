using System.Collections.Generic;
using System.Linq;
using Vokabular.ProjectParsing.Model.Entities;

namespace Vokabular.Marc21ProjectParser.DataFieldProcessors
{
    public class LiteraryGenreProcessor : IDataFieldProcessor
    {
        private const string GenreCode = "a";
        public IList<string> Tags { get; } = new List<string> {"655"};

        public void Process(dataFieldType dataField, ImportedProject importedProject)
        {
            var genreSubfield = dataField.subfield.FirstOrDefault(x => x.code == GenreCode);
            if (genreSubfield != null)
            {
                importedProject.LiteraryGenres.Add(genreSubfield.Value);
            }
        }
    }
}
