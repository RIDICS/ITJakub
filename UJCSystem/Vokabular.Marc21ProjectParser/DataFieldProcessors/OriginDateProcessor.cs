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

            project.ProjectMetadata.OriginDate = dateSubfield.Value;
            var dates = dateSubfield.Value.Split('-');

            if (int.TryParse(dates[0], out var year) && year > 0)
            {
                project.ProjectMetadata.ManuscriptDescriptionData.NotBefore = new DateTime(year, 1, 1);
            }

            if (dates.Length > 1 && dates[1].Length > 0 && int.TryParse(dates[0], out year) && year > 0)
            {
                project.ProjectMetadata.ManuscriptDescriptionData.NotAfter = new DateTime(year, 1, 1);
            }
        }
    }
}