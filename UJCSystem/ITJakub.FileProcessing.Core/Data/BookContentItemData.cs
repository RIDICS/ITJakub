using System.Collections.Generic;

namespace ITJakub.FileProcessing.Core.Data
{
    public class BookContentItemData
    {
        public string Text { get; set; }
        public List<BookContentItemData> SubContentItems { get; set; }
        public int ItemOrder { get; set; }
        public BookPageData Page { get; set; }
    }
}