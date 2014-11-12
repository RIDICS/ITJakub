using System;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.FileProcessing.Core.XSLT;

namespace ITJakub.FileProcessing.Core.Processors.Header
{
    public class RepositoryProcessor : ConcreteInstanceListProcessorBase<ManuscriptDescription>
    {
        public RepositoryProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "repository"; }
        }

        protected override void ProcessElement(ManuscriptDescription msDesc, XmlReader xmlReader)
        {
            xmlReader.Read();                           //read text value
            string value = xmlReader.Value;
            msDesc.Repository = value;
        }
    
    }
}