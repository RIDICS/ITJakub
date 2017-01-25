using System.Collections.Generic;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing.Processors.Accessories;
using ITJakub.FileProcessing.Core.XMLProcessing.Processors.Audiobooks;
using ITJakub.FileProcessing.Core.XMLProcessing.Processors.BookContent;
using ITJakub.FileProcessing.Core.XMLProcessing.Processors.Header;
using ITJakub.FileProcessing.Core.XMLProcessing.Processors.Headwords;
using ITJakub.FileProcessing.Core.XMLProcessing.Processors.Pages;
using ITJakub.FileProcessing.Core.XMLProcessing.Processors.Terms;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors
{
    public class DocumentProcessor : ProcessorBase
    {
        public DocumentProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "document"; }
        }

        public string XmlRootName
        {
            get { return NodeName; }
        }


        protected override IEnumerable<ProcessorBase> SubProcessors
        {
            get
            {
                return new List<ProcessorBase>
                {
                    Container.Resolve<TeiHeaderProcessor>(),
                    Container.Resolve<TableOfContentProcessor>(),
                    Container.Resolve<PagesProcessor>(),
                    Container.Resolve<TermsProcessor>(),
                    Container.Resolve<TracksProcessor>(),
                    Container.Resolve<HeadwordsTableProcessor>(),
                    Container.Resolve<AccessoriesProcessor>(),
                };
            }
        }

        protected override void ProcessAttributes(BookData bookData, XmlReader xmlReader)
        {
            string bookGuid = xmlReader.GetAttribute("n");
            bookData.BookXmlId = bookGuid;
            
            //string docType = xmlReader.GetAttribute("doctype");
            //BookTypeEnum bookTypeEnum;
            //Enum.TryParse(docType, true, out bookTypeEnum);
            //var bookType = m_bookRepository.FindBookType(bookTypeEnum) ?? new BookType {Type = bookTypeEnum};

            //book.BookType = bookType;
            
            string versionId = xmlReader.GetAttribute("versionId");
            bookData.VersionXmlId = versionId;
        }
    }
}