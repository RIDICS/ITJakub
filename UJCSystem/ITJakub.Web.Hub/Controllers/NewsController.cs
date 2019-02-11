using System;
using System.Collections.Generic;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Models;
using ITJakub.Web.Hub.Models.FeedResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.SyndicationFeed;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.Shared.Const;

namespace ITJakub.Web.Hub.Controllers
{
    [Authorize(Roles = CustomRole.CanAddNews)]
    public class NewsController : BaseController
    {
        private readonly GoogleCalendarConfiguration m_googleCalendarConfiguration;

        public NewsController(CommunicationProvider communicationProvider, IOptions<GoogleCalendarConfiguration> options) : base(communicationProvider)
        {
            m_googleCalendarConfiguration = options.Value;
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
                    var syndicationItem = new SyndicationItem
                    {
                        Id = feed.Id.ToString(),
                        Title = feed.Title,
                        Description = feed.Text,
                        Published = feed.CreateTime,
                        LastUpdated = feed.CreateTime,
                    };
                    var person = new SyndicationPerson($"{feed.CreatedByUser.FirstName} {feed.CreatedByUser.LastName}", feed.CreatedByUser.Email);
                    var url = new SyndicationLink(new Uri(feed.Url));
                    syndicationItem.AddContributor(person);
                    syndicationItem.AddLink(url);

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

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Calendar()
        {
            var model = new GoogleCalendarViewModel
            {
                CalendarId = m_googleCalendarConfiguration.CalendarId
            };

            return View(model);
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