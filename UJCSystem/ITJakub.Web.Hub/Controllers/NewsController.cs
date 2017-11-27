using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Core.Identity;
using ITJakub.Web.Hub.Models;
using ITJakub.Web.Hub.Models.FeedResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;

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

            using (var client = GetRestClient())
            {
                var feeds = client.GetNewsSyndicationItems(0, count, NewsTypeEnumContract.Web);
                foreach (var feed in feeds.List)
                {
                    var syndicationItem = new SyndicationItem(feed.Title, feed.Text, new Uri(feed.Url));
                    syndicationItem.PublishDate = feed.CreateTime;
                    var person = new SyndicationPerson(feed.CreatedByUser.Email) {Name = $"{feed.CreatedByUser.FirstName} {feed.CreatedByUser.LastName}"};
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
            using (var client = GetRestClient())
            {
                var feeds = client.GetNewsSyndicationItems(start, count, NewsTypeEnumContract.Web);
                return Json(feeds);
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
            using (var client = GetRestClient())
            {
                var data = new CreateNewsSyndicationItemContract
                {
                    Title = model.Title,
                    ItemType = (NewsTypeEnumContract) model.ItemType,
                    Text = model.Content,
                    Url = model.Url,
                };
                client.CreateNewsSyndicationItem(data);
            }

            return RedirectToAction("Index", "Home");
        }
    }

    public enum FeedType
    {
        Rss,
        Atom
    }
}