using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Entities.Enums;
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

        protected override void ProcessElement(BookVersion bookVersion, XmlReader xmlReader)
        {
            if (!xmlReader.HasAttributes)
            {
                bookVersion.BiblText = GetInnerContentAsString(xmlReader);
            }
            else
            {
                if (xmlReader.GetAttribute("type") == "acronym")
                {
                    switch (xmlReader.GetAttribute("subtype"))
                    {
                        case "original-text":
                            bookVersion.RelicAbbreviation = GetInnerContentAsString(xmlReader);

                            break;

                        case "source":
                            bookVersion.SourceAbbreviation = GetInnerContentAsString(xmlReader);

                            break;

                        default:
                            if (m_log.IsDebugEnabled)
                                m_log.DebugFormat("Unknown bibl subtype attribute '${0}'",
                                    xmlReader.GetAttribute("subtype"));

                            break;
                    }
                }
                else
                {
                    if (m_log.IsDebugEnabled)
                        m_log.DebugFormat("Unknown bibl type attribute '${0}'", xmlReader.GetAttribute("type"));
                }
            }
        }
    }
}