using System;
using System.Collections.Generic;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors
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

        protected override sealed T LoadInstance(BookVersion bookVersion)
        {
           throw new NotSupportedException();
        }

        protected override sealed void SaveInstance(T instance, BookVersion bookVersion)
        {
            throw new NotSupportedException();
        }
    }
}