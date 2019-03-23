using System;
using System.Collections.Generic;
using System.Linq;
using Vokabular.ProjectParsing.Model.Entities;

namespace Vokabular.Marc21ProjectParser.DataFieldProcessors
{
    public class OriginDateProcessor : IDataFieldProcessor
    {
        private const string NotAfterAndNotBeforeCode = "a";

        public IList<string> Tags { get; } = new List<string> {"648"};

        public void Process(dataFieldType dataField, Project project)
        {
            var dateSubfield = dataField.subfield.FirstOrDefault(x => x.code == NotAfterAndNotBeforeCode);
            if (dateSubfield == null)
            {
                return;
            }

            var dates = dateSubfield.Value.Split('-');
            project.ProjectMetadata.ManuscriptDescriptionData.NotBefore = new DateTime(int.Parse(dates[0]), 1, 1);
            if (dates.Length > 1 && dates[1].Length > 0 && dates[1].All(char.IsDigit))
                project.ProjectMetadata.ManuscriptDescriptionData.NotAfter = new DateTime(int.Parse(dates[1]), 1, 1);
        }
    }
}