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
                new PageBookmarkContract{ PageXmlId= "t-1.body-1.div-2.div-1.div-111.p-2.pb-1", PagePosition = 197} //TODO TEST DATA FROM LekChir
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