using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Xml;
using Microsoft.AspNetCore.Mvc;

namespace ITJakub.Web.Hub.Models.FeedResults
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
        /// <param name="requestUrl">The URL of feed alternate link.</param>
        public AtomResult(string title, List<SyndicationItem> feedItems, Uri requestUrl)
            : base("application/atom+xml")
        {
            m_feed = new SyndicationFeed(title, title, requestUrl) { Items = feedItems };
        }

        public override void ExecuteResult(ActionContext context)
        {
            using (XmlWriter writer = XmlWriter.Create(context.HttpContext.Response.Body))
            {
                m_feed.GetAtom10Formatter().WriteTo(writer);
            }
        }
    }
}