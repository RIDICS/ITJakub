using System.Collections.Generic;
using System.Linq;
using Vokabular.ProjectParsing.Model.Entities;

namespace Vokabular.Marc21ProjectParser.DataFieldProcessors
{
    public class ProjectNameProcessor : IDataFieldProcessor
    {
        private const string ProjectNameFirstLineCode = "a";
        private const string ProjectNameSecondLineCode = "b";

        public IList<string> Tags { get; } = new List<string> {"245"};

        public void Process(dataFieldType dataField, Project project)
        {
            project.Name = dataField.subfield.First(x => x.code == ProjectNameFirstLineCode).Value;
            var nameSecondLine = dataField.subfield.FirstOrDefault(x => x.code == ProjectNameSecondLineCode);

            if (nameSecondLine != null)
            {
                project.Name = string.Concat(project.Name, " ", nameSecondLine.Value);
            }

            project.Name = project.Name.RemoveUnnecessaryCharacters();
        }
    }
}