using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web;
using System.Web.Mvc;
using ITJakub.ITJakubService.DataContracts.Clients;
using ITJakub.Shared.Contracts.Notes;
using ITJakub.Web.Hub.Models;
using ITJakub.Web.Hub.Results;

namespace ITJakub.Web.Hub.Controllers
{
    [Authorize]
    public class NewsController : Controller
    {
        private readonly ItJakubServiceClient m_mainServiceClient = new ItJakubServiceClient();
        

        // GET: News
        public ActionResult Index()
        {
            //return View();
            return null;
        }
        
        [HttpGet]
        [AllowAnonymous]
        public virtual ActionResult Feed(string feedType)
        {            

            

            FeedType ft;
            if (!Enum.TryParse(feedType, true, out ft))
            {
                throw new ArgumentException("Unknown feed type");
            }

            var items = new List<SyndicationItem>();

            using (var client = new ItJakubServiceClient())
            {
                var feeds = client.GetNewsSyndicationItems(0, 10);
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
       

        public ActionResult Add()
        {
            return View("AddNews");
        }



        [HttpPost]        
        [ValidateAntiForgeryToken]
        public ActionResult Add(NewsSyndicationItemModel model)
        {
            var username = HttpContext.User.Identity.Name;
            
                using(var client = new ItJakubServiceEncryptedClient())
                 client.CreateNewsSyndicationItem(model.Title, model.Content, model.Url, username);


            return Json(new { });
        }
    }

    public enum FeedType
    {
        Rss,
        Atom,
    }
}