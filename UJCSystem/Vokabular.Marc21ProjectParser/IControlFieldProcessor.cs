using System.Collections.Generic;
using Vokabular.ProjectParsing.Model.Entities;
using Vokabular.ProjectParsing.Parsers;

namespace Vokabular.Marc21ProjectParser
{
    public interface IControlFieldProcessor : IFieldProcessor
    {
        void Process(controlFieldType dataField, Project project, IDictionary<ParserHelperTypes, string> data);
    }
}
