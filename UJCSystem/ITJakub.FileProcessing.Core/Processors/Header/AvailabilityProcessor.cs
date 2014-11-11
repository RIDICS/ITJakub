using System;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Entities.Enums;
using ITJakub.FileProcessing.Core.XSLT;

namespace ITJakub.FileProcessing.Core.Processors.Header
{
    public class AvailabilityProcessor : ListProcessorBase
    {
        public AvailabilityProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "availability"; }
        }

        protected override void ProcessElement(BookVersion bookVersion, XmlReader xmlReader)
        {
            ProcessAttributes(bookVersion, xmlReader);
            bookVersion.Copyright = GetInnerContentAsString(xmlReader);
        }

        protected override void ProcessAttributes(BookVersion bookVersion, XmlReader xmlReader)
        {
            string status = xmlReader.GetAttribute("status");
            bookVersion.AvailabilityStatus = ParseEnum<AvailabilityStatusEnum>(status);
        }
    }
}