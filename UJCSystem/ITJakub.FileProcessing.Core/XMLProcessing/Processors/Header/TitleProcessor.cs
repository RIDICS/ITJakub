using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Header
{
    public class TitleProcessor : ListProcessorBase
    {
        public TitleProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "title"; }
        }

        protected override void ProcessElement(BookVersion bookVersion, XmlReader xmlReader)
        {
            string titleType = xmlReader.GetAttribute("type");
            if (string.IsNullOrEmpty(titleType) || titleType.Equals("main"))
            {
                bookVersion.Title = GetInnerContentAsString(xmlReader);
            }
            else if (titleType.Equals("sub"))
            {
                bookVersion.SubTitle = GetInnerContentAsString(xmlReader);
            } 
        }
    }
}