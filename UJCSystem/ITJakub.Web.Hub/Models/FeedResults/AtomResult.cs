using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SyndicationFeed;
using Microsoft.SyndicationFeed.Atom;

namespace ITJakub.Web.Hub.Models.FeedResults
{
    public class AtomResult : FileResult
    {
        private readonly string m_feedTitle;
        private readonly List<SyndicationItem> m_feedItems;
        private readonly Uri m_feedRequestUrl;

        /// <summary>
        /// Creates a new instance of AtomResult
        /// </summary>
        /// <param name="feedTitle">The title for the feed.</param>
        /// <param name="feedItems">The items of the feed.</param>
        /// <param name="feedRequestUrl">The URL of feed alternate link.</param>
        public AtomResult(string feedTitle, List<SyndicationItem> feedItems, Uri feedRequestUrl)
            : base("application/atom+xml")
        {
            m_feedTitle = feedTitle;
            m_feedItems = feedItems;
            m_feedRequestUrl = feedRequestUrl;
        }

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            using (XmlWriter xmlWriter = XmlWriter.Create(context.HttpContext.Response.Body, new XmlWriterSettings { Async = true, Indent = true }))
            {
                var writer = new AtomFeedWriter(xmlWriter);

                var uniqueFeedIdBuilder = new UriBuilder(m_feedRequestUrl)
                {
                    Scheme = Uri.UriSchemeHttp,
                    Query = string.Empty
                };
                var uniqueFeedId = uniqueFeedIdBuilder.ToString();
                await writer.WriteId(uniqueFeedId);
                await writer.WriteTitle(m_feedTitle);
                //await writer.WriteDescription(m_feedTitle);
                await writer.Write(new SyndicationLink(m_feedRequestUrl));
                //await writer.WriteUpdated(DateTimeOffset.UtcNow);

                foreach (var syndicationItem in m_feedItems)
                {
                    syndicationItem.Id = $"{uniqueFeedId}?itemId={syndicationItem.Id}";
                    await writer.Write(syndicationItem);
                }

                xmlWriter.Flush();
            }
        }
    }
}