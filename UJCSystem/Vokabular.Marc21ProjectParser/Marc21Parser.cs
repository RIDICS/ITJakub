using System;
using System.Collections.Generic;
using Vokabular.ProjectParsing;
using Vokabular.ProjectParsing.Model.Entities;
using Vokabular.ProjectParsing.Parsers;

namespace Vokabular.Marc21ProjectParser
{
    public class Marc21Parser : IParser
    {
        public string ParserTypeName { get; } = "Marc21";
        private readonly IDictionary<string, IDataFieldProcessor> m_dataFieldProcessors;
        private readonly IDictionary<string, IControlFieldProcessor> m_controlFieldProcessors;

        public Marc21Parser(IEnumerable<IDataFieldProcessor> dataFieldProcessors,
            IEnumerable<IControlFieldProcessor> controlFieldProcessors)
        {
            m_dataFieldProcessors = new Dictionary<string, IDataFieldProcessor>();
            m_controlFieldProcessors = new Dictionary<string, IControlFieldProcessor>();

            foreach (var dataFieldProcessor in dataFieldProcessors)
            {
                foreach (var tag in dataFieldProcessor.Tags)
                {
                    m_dataFieldProcessors.Add(tag, dataFieldProcessor);
                }
            }

            foreach (var controlFieldProcessor in controlFieldProcessors)
            {
                foreach (var tag in controlFieldProcessor.Tags)
                {
                    m_controlFieldProcessors.Add(tag, controlFieldProcessor);
                }
            }
        }

        public ProjectImportMetadata Parse(ProjectImportMetadata projectImportMetadata, Dictionary<ParserHelperTypes, string> config)
        {
            if (projectImportMetadata.IsFaulted)
            {
                return projectImportMetadata;
            }

            try
            {
                var record = ((string)projectImportMetadata.RawData).XmlDeserializeFromString<MARC21record>();
                var project = new Project();

                foreach (var dataField in record.datafield)
                {
                    m_dataFieldProcessors.TryGetValue(dataField.tag, out var dataFieldProcessor);
                    dataFieldProcessor?.Process(dataField, project);
                }

                foreach (var controlField in record.controlfield)
                {
                    m_controlFieldProcessors.TryGetValue(controlField.tag, out var controlFieldProcessor);
                    controlFieldProcessor?.Process(controlField, project, config);
                }

                projectImportMetadata.Project = project;
            }
            catch (Exception e)
            {
                projectImportMetadata.IsFaulted = true;
                projectImportMetadata.FaultedMessage = e.Message;
            }
         
            return projectImportMetadata;
        }
    }
}