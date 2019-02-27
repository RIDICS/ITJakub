using System.Collections.Generic;
using System.Linq;
using Vokabular.ProjectParsing.Model.Entities;

namespace Vokabular.Marc21ProjectParser.DataFieldProcessors
{
    public class AuthorProcessor : IDataFieldProcessor
    {
        private const string AuthorCode = "a";
        public IList<string> Tags { get; } = new List<string> { "100", "600"};

        public void Process(dataFieldType dataField, Project project)
        {
            var author = dataField.subfield.First(x => x.code == AuthorCode).Value;

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

            project.Authors.Add(new Author(firstName, lastName));
        }
    }
}