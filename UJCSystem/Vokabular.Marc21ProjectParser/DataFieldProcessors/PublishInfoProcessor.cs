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

        public void Process(dataFieldType dataField, Project project)
        {
            var publishPlace = dataField.subfield.FirstOrDefault(x => x.code == PublishPlaceCode);
            project.MetadataResource.PublishPlace = publishPlace?.Value.RemoveUnnecessaryCharacters();

            var publisherText = dataField.subfield.FirstOrDefault(x => x.code == PublisherTextCode);
            project.MetadataResource.PublisherText = publisherText?.Value.RemoveUnnecessaryCharacters();

            project.MetadataResource.PublishDate = dataField.subfield.FirstOrDefault(x => x.code == PublishDateCode)?.Value;
        }
    }
}