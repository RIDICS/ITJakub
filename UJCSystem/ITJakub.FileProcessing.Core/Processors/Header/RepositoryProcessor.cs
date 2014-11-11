using System;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.FileProcessing.Core.XSLT;

namespace ITJakub.FileProcessing.Core.Processors.Header
{
    public class RepositoryProcessor : ListProcessorBase
    {
        public RepositoryProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "repository"; }
        }

        protected override void ProcessElement(BookVersion bookVersion, XmlReader xmlReader)
        {
            throw new NotImplementedException();
        }
    
    }
}