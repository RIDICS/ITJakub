using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using ITJakub.Shared.Contracts.News;
using ITJakub.Web.Hub.Identity;
using ITJakub.Web.Hub.Models;
using ITJakub.Web.Hub.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITJakub.Web.Hub.Controllers
{
    [Authorize(Roles = CustomRole.CanAddNews)]
    public class NewsController : BaseController
    {
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

            if (ft == FeedType.Rss)
                return new RssResult("Vokabular feed", items);

            return new AtomResult("Vokabular feed", items);
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
            var username = HttpContext.User.Identity.Name;

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