using System.Collections.Generic;
using System.Linq;
using Vokabular.ProjectParsing.Model.Entities;

namespace Vokabular.Marc21ProjectParser.DataFieldProcessors
{
    public class GenreProcessor : IDataFieldProcessor
    {
        private const string GenreCode = "a";
        public IList<string> Tags { get; } = new List<string> {"655"};

        public void Process(dataFieldType dataField, Project project)
        {
            var genre = dataField.subfield.First(x => x.code == GenreCode).Value;

            project.LiteraryGenres.Add(genre);
        }
    }
}
