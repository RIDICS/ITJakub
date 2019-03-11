using System.Collections.Generic;
using System.Linq;
using Vokabular.ProjectParsing.Model.Entities;

namespace Vokabular.Marc21ProjectParser.DataFieldProcessors
{
    public class EditionNoteProcessor : IDataFieldProcessor
    {
        private const string EditionNoteCode = "a";
        public IList<string> Tags { get; } = new List<string> {"520"};

        public void Process(dataFieldType dataField, Project project)
        {
            project.EditionNote = dataField.subfield.First(x => x.code == EditionNoteCode).Value;
        }
    }
}
