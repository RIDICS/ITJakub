using System.Collections.Generic;
using Vokabular.ProjectImport.DataEntities.Database;
using Vokabular.ProjectParsing.Model.Entities;

namespace Vokabular.ProjectParsing.Parsers
{
    public abstract class ParserBase: IParser
    {
        public ParserType ParserType { get; }

        protected ParserBase(ParserType parserType)
        {
            ParserType = parserType;
        }

        public abstract Project Parse(string xml, Dictionary<ParserHelperTypes, string> config);
    }
}
