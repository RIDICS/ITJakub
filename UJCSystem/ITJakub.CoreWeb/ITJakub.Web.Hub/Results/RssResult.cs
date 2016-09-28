using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Web;
using System.Xml;
using Microsoft.AspNetCore.Mvc;

namespace ITJakub.Web.Hub.Results
{
    public class RssResult : FileResult
    {
        private readonly SyndicationFeed m_feed;

        /// <summary>
        /// Creates a new instance of RssResult
        /// based on this sample 
        /// http://www.developerzen.com/2009/01/11/aspnet-mvc-rss-feed-action-result/
        /// </summary>
        /// <param name="feed">The feed to return the user.</param>
        public RssResult(SyndicationFeed feed)
            : base("application/rss+xml")
        {
            m_feed = feed;
        }

        /// <summary>
        /// Creates a new instance of RssResult
        /// </summary>
        /// <param name="title">The title for the feed.</param>
        /// <param name="feedItems">The items of the feed.</param>
        /// <param name="requestUrl">The URL of feed alternate link.</param>
        public RssResult(string title, List<SyndicationItem> feedItems, Uri requestUrl)
            : base("application/rss+xml")
        {
            m_feed = new SyndicationFeed(title, title, requestUrl) { Items = feedItems };
        }

        public override void ExecuteResult(ActionContext context)
        {
            using (XmlWriter writer = XmlWriter.Create(context.HttpContext.Response.Body))
            {
                m_feed.GetRss20Formatter().WriteTo(writer);
            }
        }
    }
}