using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.FileProcessing.Core.XSLT;

namespace ITJakub.FileProcessing.Core.Processors
{
    public abstract class ProcessorBase
    {
        private readonly XsltTransformationManager m_xsltTransformationManager;
        private bool m_initialized;
        private Dictionary<string, ProcessorBase> m_processors;


        protected ProcessorBase(XsltTransformationManager xsltTransformationManager, IKernel container)
        {
            m_xsltTransformationManager = xsltTransformationManager;
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
            PreprocessSetup(bookVersion);
            ProcessAttributes(bookVersion, xmlReader);
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.IsStartElement() &&
                    m_processors.ContainsKey(xmlReader.LocalName))
                {
                    m_processors[xmlReader.LocalName].Process(bookVersion, GetSubtree(xmlReader));
                }
            }
        }

        protected virtual void PreprocessSetup(BookVersion bookVersion)
        {
        }

        private XmlReader GetSubtree(XmlReader xmlReader)
        {
            XmlReader subtree = xmlReader.ReadSubtree();
            subtree.Read();
            return subtree;
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
            return m_xsltTransformationManager.TransformToString(xmlReader);
        }

        protected TE ParseEnum<TE>(string value) where TE : struct
        {
            TE enumInstance;
            Enum.TryParse(value, true, out enumInstance);
            return enumInstance;
        }

        private Dictionary<string, ProcessorBase> GetProcessors()
        {
            return SubProcessors.ToDictionary(x => x.NodeName);
        }
    }


    public abstract class ListProcessorBase : ProcessorBase
    {
        protected ListProcessorBase(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override sealed IEnumerable<ProcessorBase> SubProcessors
        {
            get { return new List<ProcessorBase>(); }
        }
    }
}