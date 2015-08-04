using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Entities.Enums;
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

        protected override void ProcessAttributes(BookVersion bookVersion, XmlReader xmlReader)
        {
            var entryId = xmlReader.GetAttribute("entryId");
            var defaultHw = xmlReader.GetAttribute("defaultHw");
            var defaultHwSorting = xmlReader.GetAttribute("defaultHw-sorting");
            var hw = xmlReader.GetAttribute("hw");
            var visibility = xmlReader.GetAttribute("visibility");
            var visibilityEnum = ParseEnum<VisibilityEnum>(visibility);
            var image = xmlReader.GetAttribute("image-name");

            var bookHeadword = new BookHeadword
            {
                BookVersion = bookVersion,
                XmlEntryId = entryId,
                DefaultHeadword = defaultHw,
                Headword = hw,
                Visibility = visibilityEnum,
                SortOrder = defaultHwSorting,
                Image = image
            };

            bookVersion.BookHeadwords.Add(bookHeadword);
        }
    }
}