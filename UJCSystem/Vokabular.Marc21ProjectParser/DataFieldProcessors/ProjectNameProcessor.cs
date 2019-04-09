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
            var projectSubfield = dataField.subfield.FirstOrDefault(x => x.code == ProjectNameFirstLineCode);
            if (projectSubfield == null)
            {
                return;
            }

            project.ProjectMetadata.Title = projectSubfield.Value;
            var nameSecondLine = dataField.subfield.FirstOrDefault(x => x.code == ProjectNameSecondLineCode);

            if (nameSecondLine != null)
            {
                project.ProjectMetadata.Title= string.Concat(project.ProjectMetadata.Title, " ", nameSecondLine.Value);
            }

            project.ProjectMetadata.Title = project.ProjectMetadata.Title.RemoveUnnecessaryCharacters();
        }
    }
}