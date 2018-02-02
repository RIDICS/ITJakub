using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SyndicationFeed;
using Microsoft.SyndicationFeed.Rss;

namespace ITJakub.Web.Hub.Models.FeedResults
{
    public class RssResult : FileResult
    {
        private readonly string m_feedTitle;
        private readonly List<SyndicationItem> m_feedItems;
        private readonly Uri m_feedRequestUrl;

        /// <summary>
        /// Creates a new instance of RssResult
        /// </summary>
        /// <param name="feedTitle">The title for the feed.</param>
        /// <param name="feedItems">The items of the feed.</param>
        /// <param name="feedRequestUrl">The URL of feed alternate link.</param>
        public RssResult(string feedTitle, List<SyndicationItem> feedItems, Uri feedRequestUrl)
            : base("application/rss+xml")
        {
            m_feedTitle = feedTitle;
            m_feedItems = feedItems;
            m_feedRequestUrl = feedRequestUrl;
        }

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            using (XmlWriter xmlWriter = XmlWriter.Create(context.HttpContext.Response.Body, new XmlWriterSettings { Async = true, Indent = true }))
            {
                var writer = new RssFeedWriter(xmlWriter);

                await writer.WriteTitle(m_feedTitle);
                await writer.WriteDescription(m_feedTitle);
                await writer.Write(new SyndicationLink(m_feedRequestUrl));
                //await writer.WritePubDate(DateTimeOffset.UtcNow);

                foreach (var syndicationItem in m_feedItems)
                {
                    syndicationItem.Id = null; // GUID in RSS is not required
                    await writer.Write(syndicationItem);
                }

                xmlWriter.Flush();
            }
        }
    }
}