using System;
using System.Collections.Generic;
using System.Linq;
using Vokabular.ProjectParsing.Model.Entities;

namespace Vokabular.Marc21ProjectParser.DataFieldProcessors
{
    public class PublishInfo2Processor : IDataFieldProcessor
    {
        private const string PublishInfoCode = "d";

        public IList<string> Tags { get; } = new List<string> {"773"};

        public void Process(dataFieldType dataField, Project project)
        {
            var publishInfo = dataField.subfield.FirstOrDefault(x => x.code == PublishInfoCode);
            if (publishInfo != null)
            {
                var index = publishInfo.Value.IndexOf(':');

                if (index > 0)
                {
                    var publishPlace = publishInfo.Value.Substring(0, index);
                    project.MetadataResource.PublishPlace = publishPlace.RemoveUnnecessaryCharacters();
                    var rest = publishInfo.Value.Substring(index);

                    if (!string.IsNullOrEmpty(rest))
                    {
                        index = rest.IndexOf(",", StringComparison.Ordinal);

                        if (index > 0)
                        {
                            var publisherText = rest.Substring(0, index);
                            var publishDate = rest.Substring(index);

                            if (!string.IsNullOrEmpty(publisherText) && publisherText.Length > 1)
                            {
                                project.MetadataResource.PublisherText = publisherText.RemoveUnnecessaryCharacters();
                            }

                            if (!string.IsNullOrEmpty(publishDate) && publishDate.Length > 1)
                            {
                                project.MetadataResource.PublishDate = publishDate.RemoveUnnecessaryCharacters();
                            }
                        }
                        else
                        {
                            if (int.TryParse(rest, out int publishDate))
                            {
                                project.MetadataResource.PublishDate = publishDate.ToString();
                            }
                            else
                            {
                                project.MetadataResource.PublisherText = rest;
                            }
                        }

                    }
                }
                else
                {
                    index = publishInfo.Value.IndexOf(',');

                    if (index > 0)
                    {
                        var publishPlace = publishInfo.Value.Substring(0, index);
                        project.MetadataResource.PublishPlace = publishPlace.RemoveUnnecessaryCharacters();
                        var rest = publishInfo.Value.Substring(index);

                        index = rest.IndexOf(";", StringComparison.Ordinal);

                        if (index > 0)
                        {
                            var publisherText = rest.Substring(0, index);
                            var publishDate = rest.Substring(index);

                            if (!string.IsNullOrEmpty(publisherText) && publisherText.Length > 1)
                            {
                                project.MetadataResource.PublisherText = publisherText.RemoveUnnecessaryCharacters();
                            }

                            if (!string.IsNullOrEmpty(publishDate) && publishDate.Length > 1)
                            {
                                project.MetadataResource.PublishDate = publishDate.RemoveUnnecessaryCharacters();
                            }
                        }
                        else
                        {
                            if (int.TryParse(rest, out int publishDate))
                            {
                                project.MetadataResource.PublishDate = publishDate.ToString();
                            }
                            else
                            {
                                project.MetadataResource.PublisherText = rest;
                            }
                        }
                    }
                }
            }
        }
    }
}