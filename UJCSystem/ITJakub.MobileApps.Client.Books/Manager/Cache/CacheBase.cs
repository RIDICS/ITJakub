using System.Collections.Generic;
using System.Threading.Tasks;

namespace ITJakub.MobileApps.Client.Books.Manager.Cache
{
    public abstract class CacheBase<T>
    {
        private readonly int m_maxSize;
        private readonly Dictionary<BookPageKey, T> m_cache;
        private readonly LinkedList<BookPageKey> m_accessHistory;

        protected CacheBase(int maxSize)
        {
            m_maxSize = maxSize;
            m_cache = new Dictionary<BookPageKey, T>(maxSize);
            m_accessHistory = new LinkedList<BookPageKey>();
        }
        
        protected abstract Task<T> GetFromServerAsync(string bookGuid, string pageId);

        public async Task<T> Get(string bookGuid, string pageId)
        {
            var key = new BookPageKey(bookGuid, pageId);
            if (!m_cache.ContainsKey(key))
            {
                if (m_cache.Count >= m_maxSize)
                    RemoveTheOldestItem();

                var item = await GetFromServerAsync(bookGuid, pageId);
                m_accessHistory.AddFirst(key);
                m_cache[key] = item;

                return item;
            }

            UpdateAccessItem(key);
            return m_cache[key];
        }

        private void RemoveTheOldestItem()
        {
            var lastItem = m_accessHistory.Last.Value;
            m_accessHistory.RemoveLast();
            m_cache.Remove(lastItem);
        }

        private void UpdateAccessItem(BookPageKey key)
        {
            m_accessHistory.Remove(key);
            m_accessHistory.AddFirst(key);
        }

        public void InvalidateCache()
        {
            m_cache.Clear();
        }
    }
}
