using System.Collections.Generic;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Entities.Enums;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Header
{
    public class BiblProcessor : ListProcessorBase
    {
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
                var bookBibl = new BookBibl
                {
                    Type = xmlReader.GetAttribute("type"),
                    SubType = xmlReader.GetAttribute("subtype"),
                };

                bookBibl.BiblType = ParseEnum<BiblTypeEnum>(bookBibl.Type);
                bookBibl.Text = GetInnerContentAsString(xmlReader);


                if (bookVersion.BookBibls == null) bookVersion.BookBibls = new List<BookBibl>();
                bookVersion.BookBibls.Add(bookBibl);
            }
        }
    }
}