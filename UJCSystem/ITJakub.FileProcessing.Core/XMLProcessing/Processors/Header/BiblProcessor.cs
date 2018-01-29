using System.Reflection;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;
using log4net;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Header
{
    public class BiblProcessor : ListProcessorBase
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        public BiblProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "bibl"; }
        }

        protected override void ProcessElement(BookData bookData, XmlReader xmlReader)
        {
            var type = xmlReader.GetAttribute("type");
            var subType = xmlReader.GetAttribute("subtype");
            var hasAttributes = xmlReader.HasAttributes;
            var content = GetInnerContentAsString(xmlReader);

            if (!hasAttributes)
            {
                bookData.BiblText = content;
            }
            else
            {
                if (type == "acronym")
                {
                    switch (subType)
                    {
                        case "original-text":
                            bookData.RelicAbbreviation = content;

                            break;

                        case "source":
                            bookData.SourceAbbreviation = content;

                            break;

                        default:
                            if (m_log.IsDebugEnabled)
                                m_log.DebugFormat("Unknown bibl subtype attribute '${0}'", subType);

                            break;
                    }
                }
                else
                {
                    if (m_log.IsDebugEnabled)
                        m_log.DebugFormat("Unknown bibl type attribute '${0}'", type);
                }
            }
        }
    }
}