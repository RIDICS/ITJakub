using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;
using log4net;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Header
{
    public class TermProcessor : ListProcessorBase
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly KeywordRepository m_keywordRepository;

        public TermProcessor(KeywordRepository keywordRepository, XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
            m_keywordRepository = keywordRepository;
        }

        protected override string NodeName
        {
            get { return "term"; }
        }

        protected override void ProcessElement(BookVersion bookVersion, XmlReader xmlReader)
        {
            var termType = xmlReader.GetAttribute("type");
            var term = GetInnerContentAsString(xmlReader);

            switch (termType)
            {
                case "literary-original":
                    var literaryOriginal = m_keywordRepository.FindLiteraryOriginalByName(term) ?? new LiteraryOriginal { Name = term };

                    if (bookVersion.LiteraryOriginals == null)
                    {
                        bookVersion.LiteraryOriginals = new List<LiteraryOriginal>();
                    }

                    bookVersion.LiteraryOriginals.Add(literaryOriginal);

                    break;

                case "literary-form":
                    var literaryKind = m_keywordRepository.FindLiteraryKindByName(term) ?? new LiteraryKind { Name = term };

                    if (bookVersion.LiteraryKinds == null)
                    {
                        bookVersion.LiteraryKinds = new List<LiteraryKind>();
                    }

                    bookVersion.LiteraryKinds.Add(literaryKind);

                    break;

                case "literary-genre":
                    var literaryGenre = m_keywordRepository.FindLiteraryGenreByName(term) ?? new LiteraryGenre { Name = term };

                    if (bookVersion.LiteraryGenres == null)
                    {
                        bookVersion.LiteraryGenres = new List<LiteraryGenre>();
                    }

                    bookVersion.LiteraryGenres.Add(literaryGenre);

                    break;

                default:
                    if (m_log.IsDebugEnabled)
                        m_log.DebugFormat("Unknown Keyword term type '${0}'", termType);

                    break;
            }

            bookVersion.Keywords.Add(new Keyword() { Text = term });
        }
    }
}