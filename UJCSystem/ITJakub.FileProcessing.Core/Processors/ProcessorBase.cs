using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Xsl;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;

namespace ITJakub.FileProcessing.Core.Processors
{
    public abstract class ProcessorBase
    {
        private bool m_initialized;
        private Dictionary<string, ProcessorBase> m_processors;


        protected ProcessorBase(IKernel container)
        {
            Container = container;
        }

        protected abstract string NodeName { get; }
        protected abstract IEnumerable<ProcessorBase> SubProcessors { get; }
        protected IKernel Container { get; private set; }

        public void Process(BookVersion bookVersion, XmlReader xmlReader)
        {
            if (!m_initialized)
            {
                Init();
            }
            ProcessElement(bookVersion, xmlReader);
        }

        protected virtual void ProcessElement(BookVersion bookVersion, XmlReader xmlReader)
        {
            ProcessAttributes(bookVersion, xmlReader);
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.IsStartElement() &&
                    m_processors.ContainsKey(xmlReader.LocalName))
                {
                    m_processors[xmlReader.LocalName].Process(bookVersion, xmlReader.ReadSubtree());
                }
            }
        }

        private void Init()
        {
            m_processors = GetProcessors();
            m_initialized = true;
        }

        protected virtual void ProcessAttributes(BookVersion bookVersion, XmlReader xmlReader)
        {
        }

        protected string GetInnerContentAsString(XmlReader xmlReader)
        {
            return ""; //TODO remove
            var stringWriter = new StringWriter();
            using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter))
            {
                var xslt = new XslCompiledTransform();
                xslt.Load("xmlPath"); //TODO load xslt transformation for reading string from 'w', 'pc' and 'c' elements
                xslt.Transform(xmlReader, xmlWriter);
            }
            return stringWriter.ToString();
        }

        private Dictionary<string, ProcessorBase> GetProcessors()
        {
            return SubProcessors.ToDictionary(x => x.NodeName);
        }
    }


    public abstract class ListProcessorBase : ProcessorBase
    {
        protected ListProcessorBase(IKernel container) : base(container)
        {
        }

        protected override sealed IEnumerable<ProcessorBase> SubProcessors
        {
            get { return new List<ProcessorBase>(); }
        }
    }
}