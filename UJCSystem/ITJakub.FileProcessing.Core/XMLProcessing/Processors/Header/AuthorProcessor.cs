using System.Collections.Generic;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Header
{
    public class AuthorProcessor : ListProcessorBase
    {
        public AuthorProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "author"; }
        }

        protected override void PreprocessSetup(BookData bookData)
        {
            if (bookData.Authors == null)
            {
                bookData.Authors = new List<AuthorData>();
            }
        }

        protected override void ProcessElement(BookData bookData, XmlReader xmlReader)
        {
            string name = GetInnerContentAsString(xmlReader);
            AuthorData author = new AuthorData {Name = name};
            bookData.Authors.Add(author);
        }
    }
}