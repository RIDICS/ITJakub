using System.Collections.Generic;
using Vokabular.ProjectParsing.Model.Entities;
using Vokabular.ProjectParsing.Parsers;

namespace Vokabular.Marc21ProjectParser.ControlFieldProcessors
{
    public class OriginalResourceUrlProcessor : IControlFieldProcessor
    {
        public IList<string> Tags { get; } = new List<string> {"001"};

        public void Process(controlFieldType dataField, Project project, IDictionary<ParserHelperTypes, string> data)
        {
            data.TryGetValue(ParserHelperTypes.TemplateUrl, out var urlTemplate);

            if (urlTemplate != null)
            {
                project.MetadataResource.OriginalResourceUrl = urlTemplate.Replace("{id}", dataField.Value);
            }
        }
    }
}