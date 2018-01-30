using System.Xml;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Headwords
{
    public class HeadwordProcessor : ListProcessorBase
    {
        public HeadwordProcessor(XsltTransformationManager xsltTransformationManager, IKernel container) : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "headword"; }
        }

        protected override void ProcessAttributes(BookData bookData, XmlReader xmlReader)
        {
            var entryId = xmlReader.GetAttribute("entryId");
            var defaultHw = xmlReader.GetAttribute("defaultHw");
            var defaultHwSorting = xmlReader.GetAttribute("defaultHw-sorting");
            var hw = xmlReader.GetAttribute("hw");
            var hwOriginal = xmlReader.GetAttribute("hw-original");
            var transliterated = xmlReader.GetAttribute("hw-transliterated");
            var visibility = xmlReader.GetAttribute("visibility");
            var visibilityEnum = ParseEnum<VisibilityEnum>(visibility);
            var image = xmlReader.GetAttribute("facs");

            var bookHeadword = new BookHeadwordData
            {
                XmlEntryId = entryId,
                DefaultHeadword = defaultHw,
                Headword = hw,
                HeadwordOriginal = hwOriginal,
                Transliterated = transliterated,
                Visibility = visibilityEnum,
                SortOrder = defaultHwSorting,
                Image = image
            };

            bookData.BookHeadwords.Add(bookHeadword);
        }
    }
}