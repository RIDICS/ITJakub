﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web;
using System.Web.Mvc;
using System.Xml;

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
        public RssResult(string title, List<SyndicationItem> feedItems)
            : base("application/rss+xml")
        {
            m_feed = new SyndicationFeed(title, title, HttpContext.Current.Request.Url) { Items = feedItems };
        }

        protected override void WriteFile(HttpResponseBase response)
        {
            using (XmlWriter writer = XmlWriter.Create(response.OutputStream))
            {
                m_feed.GetRss20Formatter().WriteTo(writer);                                
            }
        }
    }
}