﻿using System;
using System.Collections.Generic;
using System.Linq;
using Vokabular.ProjectParsing.Model.Entities;

namespace Vokabular.Marc21ProjectParser.DataFieldProcessors
{
    public class PublishInfo2Processor : IDataFieldProcessor
    {
        private const string PublishInfoCode = "d";

        public IList<string> Tags { get; } = new List<string> {"773"};

        public void Process(dataFieldType dataField, ImportedProject importedProject)
        {
            var publishInfo = dataField.subfield.FirstOrDefault(x => x.code == PublishInfoCode);
            if (publishInfo == null)
            {
                return;
            }

            var index = publishInfo.Value.IndexOfAny(new[] {':', ','});
            if (index <= 0)
            {
                return;
            }

            var publishPlace = publishInfo.Value.Substring(0, index);
            importedProject.ProjectMetadata.PublishPlace = publishPlace.RemoveUnnecessaryCharacters();
            var rest = publishInfo.Value.Substring(index);

            if (string.IsNullOrEmpty(rest))
            {
                return;
            }

            rest = rest.RemoveUnnecessaryCharacters();
            index = rest.LastIndexOf(',');

            if (index > 0)
            {
                var publisherText = rest.Substring(0, index);
                var publishDate = rest.Substring(index);

                if (!string.IsNullOrEmpty(publisherText) && publisherText.Length > 1)
                {
                    importedProject.ProjectMetadata.PublisherText = publisherText.RemoveUnnecessaryCharacters();
                }

                if (!string.IsNullOrEmpty(publishDate) && publishDate.Length > 1)
                {
                    importedProject.ProjectMetadata.PublishDate = publishDate.RemoveUnnecessaryCharacters();
                }
            }
            else
            {
                if (int.TryParse(rest, out var publishDate))
                {
                    importedProject.ProjectMetadata.PublishDate = publishDate.ToString();
                }
                else
                {
                    importedProject.ProjectMetadata.PublisherText = rest;
                }
            }
        }
    }
}