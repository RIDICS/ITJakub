namespace ITJakub.MobileApps.Client.Books.Manager.Cache
{
    public class BookPageKey
    {
        public BookPageKey(string bookGuid, string pageId)
        {
            BookGuid = bookGuid;
            PageId = pageId;
        }

        public string BookGuid { get; set; }

        public string PageId { get; set; }

        protected bool Equals(BookPageKey other)
        {
            return string.Equals(BookGuid, other.BookGuid) && string.Equals(PageId, other.PageId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((BookPageKey)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (BookGuid.GetHashCode() * 397) ^ PageId.GetHashCode();
            }
        }
    }
}