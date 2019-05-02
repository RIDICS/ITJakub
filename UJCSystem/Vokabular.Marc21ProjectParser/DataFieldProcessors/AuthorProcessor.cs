using System.Collections.Generic;
using System.Linq;
using Vokabular.ProjectParsing.Model.Entities;

namespace Vokabular.Marc21ProjectParser.DataFieldProcessors
{
    public class AuthorProcessor : IDataFieldProcessor
    {
        private const string AuthorCode = "a";
        public IList<string> Tags { get; } = new List<string> { "100"};

        public void Process(dataFieldType dataField, ImportedProject importedProject)
        {
            var authorSubfield = dataField.subfield.FirstOrDefault(x => x.code == AuthorCode);

            if (authorSubfield == null)
            {
                return;
            }

            var author = authorSubfield.Value;
            var lastName = author;
            var firstName = "";
            var index = author.IndexOf(',');

            if (index > 0)
            {
                lastName = author.Substring(0, index);
                if (index + 2 < author.Length)
                {
                    firstName = author.Substring(index + 2);
                    firstName = firstName.Replace(",", string.Empty);
                }
            }

            importedProject.Authors.Add(new Author(firstName, lastName));
        }
    }
}