using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.XSLT;

namespace ITJakub.FileProcessing.Core.Processors
{
    public abstract class ConcreteInstanceProcessorBase<T> : ProcessorBase
    {
        private Dictionary<string, ConcreteInstanceProcessorBase<T>> m_concreteInstaceProcessors;
        private bool m_initialized;

        protected ConcreteInstanceProcessorBase(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected abstract IEnumerable<ConcreteInstanceProcessorBase<T>> ConcreteSubProcessors { get; }

        protected override sealed IEnumerable<ProcessorBase> SubProcessors
        {
            get { return new List<ProcessorBase>(); }
        }

        protected virtual void PreprocessSetup(T bookVersion)
        {
        }

        protected virtual void ProcessAttributes(T bookVersion, XmlReader xmlReader)
        {
        }

        protected virtual void ProcessElement(T instance, XmlReader xmlReader)
        {
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.IsStartElement() &&
                    m_concreteInstaceProcessors.ContainsKey(xmlReader.LocalName))
                {
                    m_concreteInstaceProcessors[xmlReader.LocalName].Process(instance, GetSubtree(xmlReader));
                }
            }
        }

        public void Process(T instance, XmlReader xmlReader)
        {
            if (!m_initialized)
            {
                Init();
            }
            PreprocessSetup(instance);
            ProcessAttributes(instance, xmlReader);
            ProcessElement(instance, xmlReader);
        }

        private void Init()
        {
            m_concreteInstaceProcessors = GetConcreteProcessors();
            m_initialized = true;
        }

        private Dictionary<string, ConcreteInstanceProcessorBase<T>> GetConcreteProcessors()
        {
            return ConcreteSubProcessors.ToDictionary(x => x.NodeName);
        }
    }
}