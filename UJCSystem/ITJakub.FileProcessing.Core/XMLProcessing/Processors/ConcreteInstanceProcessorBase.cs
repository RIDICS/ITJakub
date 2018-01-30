using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors
{
    public abstract class ConcreteInstanceProcessorBase<T> : ProcessorBase
    {
        private Dictionary<string, ConcreteInstanceProcessorBase<T>> m_concreteInstaceProcessors;        

        protected ConcreteInstanceProcessorBase(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected abstract IEnumerable<ConcreteInstanceProcessorBase<T>> ConcreteSubProcessors { get; }

        protected override sealed IEnumerable<ProcessorBase> SubProcessors
        {
            get { return new List<ProcessorBase>(); }
        }

        protected virtual void PreprocessSetup(T instance)
        {
        }

        protected virtual void ProcessAttributes(T instance, XmlReader xmlReader)
        {
        }

        protected override void ProcessElement(BookData bookData, XmlReader xmlReader)
        {
            var instance = LoadInstance(bookData);
            Process(bookData, instance, xmlReader);
            SaveInstance(instance, bookData);
        }

        protected virtual T LoadInstance(BookData bookData)
        {
            return default(T);
        }

        protected virtual void SaveInstance(T instance, BookData bookData)
        {
        }

        protected virtual void ProcessElement(BookData bookData, T instance, XmlReader xmlReader)
        {     
            Init();    

            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.IsStartElement() &&
                    m_concreteInstaceProcessors.ContainsKey(xmlReader.LocalName))
                {
                    m_concreteInstaceProcessors[xmlReader.LocalName].Process(bookData, instance, GetSubtree(xmlReader));
                }
            }
        }

        public void Process(BookData bookData, T instance, XmlReader xmlReader)
        {          
            Init();

            PreprocessSetup(instance);
            ProcessAttributes(instance, xmlReader);
            ProcessElement(bookData, instance, xmlReader);
        }
        

        protected  override  void InitializeProcessors()
        {
            m_concreteInstaceProcessors = ConcreteSubProcessors.ToDictionary(x => x.NodeName);
            base.InitializeProcessors();
        }
    }
}