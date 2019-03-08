using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Vokabular.ProjectParsing;
using Vokabular.ProjectParsing.Model.Entities;

namespace Vokabular.Marc21ProjectParser.Filtering
{
    public class Marc21ProjectFilter : IProjectFilter
    {
        public string BibliographicFormatName { get; } = "Marc21";

        public ProjectImportMetadata Filter(ProjectImportMetadata projectImportMetadata,
            IDictionary<string, List<string>> filteringExpressions)
        {
            if (projectImportMetadata.IsFaulted)
            {
                return projectImportMetadata;
            }

            try
            {
                var record = ((string) projectImportMetadata.RawData).XmlDeserializeFromString<MARC21record>();

                foreach (var dataField in record.datafield)
                {
                    foreach (var subField in dataField.subfield)
                    {
                        filteringExpressions.TryGetValue(dataField.tag + subField.code, out var filterExpressions);
                        if (filterExpressions == null)
                        {
                            continue;
                        }

                        foreach (var expression in filterExpressions)
                        {
                            var expr = expression.Replace("%", ".*");
                            expr = Regex.Escape(expr);
                            if (Regex.IsMatch(subField.Value, expr))
                            {
                                projectImportMetadata.IsSuitable = true;
                                return projectImportMetadata;
                            }
                        }
                    }
                }

                foreach (var controlField in record.controlfield)
                {
                    filteringExpressions.TryGetValue(controlField.tag, out var filterExpressions);
                    if (filterExpressions == null)
                    {
                        continue;
                    }

                    foreach (var expression in filterExpressions)
                    {
                        var expr = expression.Replace("%", ".*");
                        expr = Regex.Escape(expr);
                        if (Regex.IsMatch(controlField.Value, expr))
                        {
                            projectImportMetadata.IsSuitable = true;
                            return projectImportMetadata;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                projectImportMetadata.IsFaulted = true;
                projectImportMetadata.FaultedMessage = e.Message;
            }

            projectImportMetadata.IsSuitable = false;
            return projectImportMetadata;
        }
    }
}