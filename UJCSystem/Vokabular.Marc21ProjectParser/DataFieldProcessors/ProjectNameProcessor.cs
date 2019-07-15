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

        public void Process(dataFieldType dataField, ImportedProject importedProject)
        {
            var projectSubfield = dataField.subfield.FirstOrDefault(x => x.code == ProjectNameFirstLineCode);
            if (projectSubfield == null)
            {
                return;
            }

            importedProject.ProjectMetadata.Title = projectSubfield.Value;
            var nameSecondLine = dataField.subfield.FirstOrDefault(x => x.code == ProjectNameSecondLineCode);

            if (nameSecondLine != null)
            {
                importedProject.ProjectMetadata.Title= string.Concat(importedProject.ProjectMetadata.Title, " ", nameSecondLine.Value);
            }

            importedProject.ProjectMetadata.Title = importedProject.ProjectMetadata.Title.RemoveUnnecessaryCharacters();
        }
    }
}