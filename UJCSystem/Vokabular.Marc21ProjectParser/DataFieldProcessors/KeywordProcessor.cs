﻿using System.Collections.Generic;
using System.Linq;
using Vokabular.ProjectParsing.Model.Entities;

namespace Vokabular.Marc21ProjectParser.DataFieldProcessors
{
    public class KeywordProcessor : IDataFieldProcessor
    {
        private const string KeywordCode = "a";
        public IList<string> Tags { get; } = new List<string> {"653"};

        public void Process(dataFieldType dataField, ImportedProject importedProject)
        {
            var keywordSubfield = dataField.subfield.FirstOrDefault(x => x.code == KeywordCode);
            if (keywordSubfield != null)
            {
                importedProject.Keywords.Add(keywordSubfield.Value);
            }
        }
    }
}