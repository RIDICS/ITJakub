using System;
using System.Collections.Generic;
using ITJakub.Web.Hub.Constants;
using ITJakub.Web.Hub.Core;
using ITJakub.Web.Hub.DataContracts;
using ITJakub.Web.Hub.Models;
using ITJakub.Web.Hub.Models.Config;
using ITJakub.Web.Hub.Models.FeedResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.SyndicationFeed;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.RestClient.Results;
using Vokabular.Shared.Const;

namespace ITJakub.Web.Hub.Controllers
{
    [Authorize(VokabularPermissionNames.AddNews)]
    public class NewsController : BaseController
    {
        private readonly GoogleCalendarConfiguration m_googleCalendarConfiguration;

        public NewsController(ControllerDataProvider controllerDataProvider, IOptions<GoogleCalendarConfiguration> options) : base(
            controllerDataProvider)
        {
            m_googleCalendarConfiguration = options.Value;
        }

        [HttpGet]
        [AllowAnonymous]
        public virtual ActionResult Feed(string feedType, int feedCount = PageSizes.NewsFeed)
        {
            FeedType ft;
            if (!Enum.TryParse(feedType, true, out ft))
            {
                throw new ArgumentException("Unknown feed type");
            }

            if (feedCount <= 0)
            {
                throw new ArgumentException("Invalid feed count");
            }


            var items = new List<SyndicationItem>();

            var client = GetNewsClient();
            {
                var feeds = client.GetNewsSyndicationItems(0, feedCount, NewsTypeEnumContract.Web, PortalTypeValue);
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
                    var person = new SyndicationPerson($"{feed.CreatedByUser.FirstName} {feed.CreatedByUser.LastName}",
                        feed.CreatedByUser.Email);
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
            var client = GetNewsClient();
            var feeds = client.GetNewsSyndicationItems(start, count, NewsTypeEnumContract.Web, PortalTypeValue);
            
            var result = new PagedResultList<NewsSyndicationItemExtendedContract>
            {
                TotalCount = feeds.TotalCount,
                List = Mapper.Map<List<NewsSyndicationItemExtendedContract>>(feeds.List),
            };

            return Json(result);
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
            var client = GetNewsClient();
            var data = new CreateNewsSyndicationItemContract
            {
                Title = model.Title,
                ItemType = NewsTypeEnumContract.Combined,
                Text = model.Content,
                Url = model.Url,
                PortalType = PortalTypeValue,
            };
            client.CreateNewsSyndicationItem(data);

            return RedirectToAction("Index", "Home");
        }
    }

    public enum FeedType
    {
        Rss,
        Atom
    }
}