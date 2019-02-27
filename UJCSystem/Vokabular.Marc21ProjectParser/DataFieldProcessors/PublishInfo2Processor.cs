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
                var index = publishInfo.Value.IndexOf(':'); //kdyz neni, tak zbytek jestli je n2jak7 tak je bud rok (cisla nebo mesto)
                        
                if (index > 0)
                {
                    var publishPlace = publishInfo.Value.Substring(0, index);
                    var rest = publishInfo.Value.Substring(index);

                    if (!string.IsNullOrEmpty(rest))
                    {
                        index = rest.IndexOf(",", StringComparison.Ordinal);//TODO kdz6 nen9 tak zbytek je text, kdyz neobsahuje cilsa, kdyz obsahuje je to text
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

                    project.MetadataResource.PublishPlace = publishPlace.RemoveUnnecessaryCharacters();
                }
            }
            

            //Praha: Horizont
            //Brno: Muzejní a vlastivědná společnost v Brně, 1990
        }
    }
}