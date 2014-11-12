using System;
using System.Collections.Generic;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.FileProcessing.Core.XSLT;

namespace ITJakub.FileProcessing.Core.Processors
{
    public abstract class ConcreteInstanceListProcessorBase<T> : ConcreteInstanceProcessorBase<T>
    {
        protected ConcreteInstanceListProcessorBase(XsltTransformationManager xsltTransformationManager,
            IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override sealed IEnumerable<ConcreteInstanceProcessorBase<T>> ConcreteSubProcessors
        {
            get { return new List<ConcreteInstanceProcessorBase<T>>(); }
        }

        protected override sealed void ProcessElement(BookVersion bookVersion, XmlReader xmlReader)
        {
            throw new NotSupportedException();
            //processor for processing concrete subinstance cannot save to BookVersion instance
        }
    }
}