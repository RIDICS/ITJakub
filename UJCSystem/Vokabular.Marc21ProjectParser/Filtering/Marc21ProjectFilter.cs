using System.Collections.Generic;
using System.Text.RegularExpressions;
using Vokabular.ProjectParsing;
using Vokabular.ProjectParsing.Model.Entities;

namespace Vokabular.Marc21ProjectParser.Filtering
{
    public class Marc21ProjectFilter : IProjectFilter
    {
        public string BibliographicFormatName { get; } = "Marc21";

        public bool Filter(ProjectImportMetadata projectImportMetadata,
            IDictionary<string, List<string>> filteringExpressions)
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
                        var expr = Regex.Escape(expression);
                        expr = expr.Replace("%", ".*");

                        if (Regex.IsMatch(subField.Value, expr))
                        {
                            return true;
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
                        return true;
                    }
                }
            }

            return false;
        }
    }
}