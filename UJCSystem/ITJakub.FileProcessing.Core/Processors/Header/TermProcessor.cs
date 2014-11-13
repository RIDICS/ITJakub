using System;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.FileProcessing.Core.XSLT;

namespace ITJakub.FileProcessing.Core.Processors.Header
{
    public class TermProcessor : ListProcessorBase
    {
        public TermProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "term"; }
        }

        protected override void ProcessElement(BookVersion bookVerison, XmlReader xmlReader)
        {
            var term = GetInnerContentAsString(xmlReader);
            bookVerison.Keywords.Add(new Keyword() {Text = term});
        }
    }
}