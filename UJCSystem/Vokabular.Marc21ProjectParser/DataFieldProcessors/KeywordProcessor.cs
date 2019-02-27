using System.Collections.Generic;
using System.Linq;
using Vokabular.ProjectParsing.Model.Entities;

namespace Vokabular.Marc21ProjectParser.DataFieldProcessors
{
    public class KeywordProcessor : IDataFieldProcessor
    {
        private const string KeywordCode = "a";
        public IList<string> Tags { get; } = new List<string> {"653"};

        public void Process(dataFieldType dataField, Project project)
        {
            project.Keywords.Add(new Keyword(dataField.subfield.First(x => x.code == KeywordCode).Value));
        }
    }
}