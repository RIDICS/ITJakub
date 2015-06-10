using System.CodeDom;
using System.Collections.Generic;
using ITJakub.ITJakubService.DataContracts;

namespace ITJakub.ITJakubService.Core
{
    public class FavoriteManager
    {
        public List<PageBookmarkContract> GetPageBookmarks(string bookId, string userName)
        {
            return new List<PageBookmarkContract>
            {
                new PageBookmarkContract{Test="TESTOVACI STRING PRO ZALOZKY"}
            };

        }

        public void AddBookmark(string bookId, string pageName, string userName)
        {
            return;
        }

        public void RemoveBookmark(string bookId, string pageName, string userName)
        {
            return;
        }
    }
}