using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace ITJakub.Web.Hub.Results
{
    public class AtomResult : FileResult
    {
        private readonly SyndicationFeed m_feed;

        /// <summary>
        /// Creates a new instance of AtomResult
        /// based on this sample 
        /// http://www.developerzen.com/2009/01/11/aspnet-mvc-rss-feed-action-result/
        /// </summary>
        /// <param name="feed">The feed to return the user.</param>
        public AtomResult(SyndicationFeed feed)
            : base("application/atom+xml")
        {
            m_feed = feed;
        }

        /// <summary>
        /// Creates a new instance of AtomResult
        /// </summary>
        /// <param name="title">The title for the feed.</param>
        /// <param name="feedItems">The items of the feed.</param>
        public AtomResult(string title, List<SyndicationItem> feedItems)
            : base("application/atom+xml")
        {
            m_feed = new SyndicationFeed(title, title, HttpContext.Current.Request.Url) { Items = feedItems };
        }


        protected override void WriteFile(HttpResponseBase response)
        {
            using (XmlWriter writer = XmlWriter.Create(response.OutputStream))
            {
                m_feed.GetAtom10Formatter().WriteTo(writer);
            }
        }
    }
}