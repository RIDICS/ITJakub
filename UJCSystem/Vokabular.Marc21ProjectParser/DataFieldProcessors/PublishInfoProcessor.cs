using System.Collections.Generic;
using System.Linq;
using Vokabular.ProjectParsing.Model.Entities;

namespace Vokabular.Marc21ProjectParser.DataFieldProcessors
{
    public class PublishInfoProcessor : IDataFieldProcessor
    {
        private const string PublishPlaceCode = "a";
        private const string PublisherTextCode = "b";
        private const string PublishDateCode = "c";

        public IList<string> Tags { get; } = new List<string> {"260", "264"};

        public void Process(dataFieldType dataField, ImportedProject importedProject)
        {
            var publishPlace = dataField.subfield.FirstOrDefault(x => x.code == PublishPlaceCode);
            importedProject.ProjectMetadata.PublishPlace = publishPlace?.Value.RemoveUnnecessaryCharacters();

            var publisherText = dataField.subfield.FirstOrDefault(x => x.code == PublisherTextCode);
            importedProject.ProjectMetadata.PublisherText = publisherText?.Value.RemoveUnnecessaryCharacters();

            importedProject.ProjectMetadata.PublishDate = dataField.subfield.FirstOrDefault(x => x.code == PublishDateCode)?.Value;
        }
    }
}