using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using ITJakub.Shared.Contracts.News;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Core.Identity;
using ITJakub.Web.Hub.Models;
using ITJakub.Web.Hub.Models.FeedResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace ITJakub.Web.Hub.Controllers
{
    [Authorize(Roles = CustomRole.CanAddNews)]
    public class NewsController : BaseController
    {
        public NewsController(CommunicationProvider communicationProvider) : base(communicationProvider)
        {
        }

        [HttpGet]
        [AllowAnonymous]
        public virtual ActionResult Feed(string feedType, string feedCount = "10")
        {
            FeedType ft;
            if (!Enum.TryParse(feedType, true, out ft))
            {
                throw new ArgumentException("Unknown feed type");
            }
            var count = Convert.ToInt32(feedCount);
            if (count <= 0)
            {
                throw new ArgumentException("Invalid feed count");
            }


            var items = new List<SyndicationItem>();

            using (var client = GetMainServiceClient())
            {
                var feeds = client.GetWebNewsSyndicationItems(0, count);
                foreach (var feed in feeds)
                {
                    var syndicationItem = new SyndicationItem(feed.Title, feed.Text, new Uri(feed.Url));
                    syndicationItem.PublishDate = feed.CreateDate;
                    var person = new SyndicationPerson(feed.UserEmail) {Name = $"{feed.UserFirstName} {feed.UserLastName}"};
                    syndicationItem.Authors.Add(person);

                    items.Add(syndicationItem);
                }
            }

            var requestUrl = new Uri(Request.GetDisplayUrl());
            switch (ft)
            {
                case FeedType.Rss:
                    return new RssResult("Vokabular feed", items, requestUrl);
                default:
                    return new AtomResult("Vokabular feed", items, requestUrl);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public virtual ActionResult GetSyndicationItems(int start, int count)
        {
            using (var client = GetMainServiceClient())
            {
                var feeds = client.GetWebNewsSyndicationItems(start, count);
                return Json(feeds);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public virtual ActionResult GetSyndicationItemCount()
        {
            using (var client = GetMainServiceClient())
            {
                var feedCount = client.GetWebNewsSyndicationItemCount();
                return Json(feedCount);
            }
        }


        public ActionResult Add()
        {
            return View("AddNews");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(NewsSyndicationItemViewModel model)
        {
            var username = GetUserName();

            using (var client = GetMainServiceClient())
                client.CreateNewsSyndicationItem(model.Title, model.Content, model.Url, (NewsTypeContract) model.ItemType, username);

            return RedirectToAction("Index", "Home");
        }
    }

    public enum FeedType
    {
        Rss,
        Atom
    }
}