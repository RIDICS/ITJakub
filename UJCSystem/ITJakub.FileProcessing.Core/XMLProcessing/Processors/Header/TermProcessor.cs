using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;
using log4net;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Header
{
    public class TermProcessor : ListProcessorBase
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        public TermProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "term"; }
        }

        protected override void ProcessElement(BookData bookData, XmlReader xmlReader)
        {
            var termType = xmlReader.GetAttribute("type");
            var term = GetInnerContentAsString(xmlReader);

            switch (termType)
            {
                case "literary-original":
                    var literaryOriginal = term;

                    if (bookData.LiteraryOriginals == null)
                    {
                        bookData.LiteraryOriginals = new List<string>();
                    }

                    bookData.LiteraryOriginals.Add(literaryOriginal);

                    break;

                case "literary-form":
                    var literaryKind = term;

                    if (bookData.LiteraryKinds == null)
                    {
                        bookData.LiteraryKinds = new List<string>();
                    }

                    bookData.LiteraryKinds.Add(literaryKind);

                    break;

                case "literary-genre":
                    var literaryGenre = term;

                    if (bookData.LiteraryGenres == null)
                    {
                        bookData.LiteraryGenres = new List<string>();
                    }

                    bookData.LiteraryGenres.Add(literaryGenre);

                    break;

                default:
                    if (m_log.IsDebugEnabled)
                        m_log.DebugFormat("Unknown Keyword term type '${0}'", termType);

                    break;
            }

            bookData.Keywords.Add(term);
        }
    }
}