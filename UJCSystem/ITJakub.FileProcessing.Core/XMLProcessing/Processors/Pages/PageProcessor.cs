using System;
using System.Reflection;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;
using log4net;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Pages
{
    public class PageProcessor : ListProcessorBase
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public PageProcessor(XsltTransformationManager xsltTransformationManager, IKernel container) : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "page"; }
        }

        protected override void ProcessAttributes(BookVersion bookVersion, XmlReader xmlReader)
        {
            var position = bookVersion.BookPages.Count + 1;
            var facsValue = xmlReader.GetAttribute("facs");
            var pageNameValue = xmlReader.GetAttribute("n") ?? Convert.ToString(position);
            var pageIdValue = xmlReader.GetAttribute("id", XmlNamespace.NamespaceName) ?? Convert.ToString(Guid.NewGuid());
            var xmlResourceValue = xmlReader.GetAttribute("resource");

            if (string.IsNullOrEmpty(xmlResourceValue) && m_log.IsFatalEnabled)
                m_log.ErrorFormat("Metadata_processor : Page in position {0} does not have resource attribute",
                    position);

            bookVersion.BookPages.Add(new BookPage
            {
                Position = position,
                Text = pageNameValue,
                BookVersion = bookVersion,
                Image = facsValue,
                XmlId = pageIdValue,
                XmlResource = xmlResourceValue
            });
        }
    }
}
