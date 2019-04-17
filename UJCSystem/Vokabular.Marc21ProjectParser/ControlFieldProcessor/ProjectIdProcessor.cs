using System.Collections.Generic;
using Vokabular.ProjectParsing.Model.Entities;

namespace Vokabular.Marc21ProjectParser.ControlFieldProcessor
{
    public class ProjectIdProcessor : IControlFieldProcessor
    {
        public IList<string> Tags { get; } = new List<string> { "001"};

        public void Process(controlFieldType dataField, Project project)
        {
            project.Id = dataField.Value;
        }
    }
}