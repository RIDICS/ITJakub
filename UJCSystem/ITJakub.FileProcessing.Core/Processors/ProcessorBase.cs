using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace ITJakub.FileProcessing.Core.Processors
{
    public abstract class ProcessorBase
    {
        private readonly Dictionary<string, ProcessorBase> m_processors;


        protected ProcessorBase()
        {
            m_processors = GetProcessors();
        }

        protected abstract string NodeName { get; }
        protected abstract IEnumerable<ProcessorBase> SubProcessors { get; }

        public virtual void Process(BookVersion bookVersion, XmlTextReader xmlReader)
        {
            ProcessAttributes(bookVersion, xmlReader);
            string nodeName = "aaa";
            if (m_processors.ContainsKey(nodeName))
            {
                m_processors[nodeName].Process(bookVersion, node.subTree());
            }
        }

        protected virtual void ProcessAttributes(BookVersion bookVersion, XmlTextReader xmlReader)
        {
        }

        protected string GetInnerContentAsString(XmlReader xmlReader)
        {
            return "aaa"; //TODO read string from 'w', 'pc' and 'c' elements
        }

        private Dictionary<string, ProcessorBase> GetProcessors()
        {
            return SubProcessors.ToDictionary(x => x.NodeName);
        }
    }


    public abstract class ListProcessorBase : ProcessorBase
    {
        protected sealed override IEnumerable<ProcessorBase> SubProcessors
        {
            get { return new List<ProcessorBase>(); }
        }
    }
}