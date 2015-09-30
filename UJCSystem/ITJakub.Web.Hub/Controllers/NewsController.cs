using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.ServiceModel.Security;
using System.ServiceModel.Syndication;
using System.Web.Mvc;
using ITJakub.ITJakubService.DataContracts.Clients;
using ITJakub.Shared.Contracts.News;
using ITJakub.Web.Hub.Identity;
using ITJakub.Web.Hub.Models;
using ITJakub.Web.Hub.Results;

namespace ITJakub.Web.Hub.Controllers
{
    public abstract class BaseController : Controller
    {


        protected ItJakubServiceAuthenticatedClient GetAuthenticatedClient()
        {
            var client = new ItJakubServiceAuthenticatedClient();
            if (client.ClientCredentials == null)
            {
                throw new ArgumentException("Cannot set credentials for client");
            }
            client.ClientCredentials.UserName.UserName = GetUserName();
            client.ClientCredentials.UserName.Password = GetCommunicationToken();

            return client;
        }

        protected ItJakubServiceEncryptedClient GetEncryptedClient()
        {
            var client = new ItJakubServiceEncryptedClient();          
            return client;
        }

        protected ItJakubServiceStreamedClient GetStreamingClient()
        {
            var client = new ItJakubServiceStreamedClient();
            return client;
        }

        protected ItJakubServiceClient GetUnsecuredClient()
        {
            var client = new ItJakubServiceClient();
            return client;
        }



        private string GetUserName()
        {
            return User.Identity.Name;
        }

        private string GetCommunicationToken()
        {
            var communicationToken = ClaimsPrincipal.Current.Claims.FirstOrDefault(x => x.Type == CustomClaimType.CommunicationToken);
            if(communicationToken == null)
                throw new ArgumentException("Cannot find communicationToken");

            return communicationToken.Value;
        }
    }

    [Authorize]
    public class NewsController : Controller
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
            int count = Convert.ToInt32(feedCount);
            if (count <= 0)
            {
                throw new ArgumentException("Invalid feed count");
            }


            var items = new List<SyndicationItem>();

            using (var client = new ItJakubServiceClient())
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
            using (var client = new ItJakubServiceClient())
            {
                List<NewsSyndicationItemContract> feeds = client.GetWebNewsSyndicationItems(start, count);
                return Json(feeds, JsonRequestBehavior.AllowGet);
            }         
        }

        [HttpGet]
        [AllowAnonymous]
        public virtual ActionResult GetSyndicationItemCount()
        {
            using (var client = new ItJakubServiceClient())
            {
                int feedCount = client.GetWebNewsSyndicationItemCount();
                return Json(feedCount, JsonRequestBehavior.AllowGet);
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

                using(var client = new ItJakubServiceEncryptedClient())
                 client.CreateNewsSyndicationItem(model.Title, model.Content, model.Url, (NewsTypeContract) model.ItemType, username);

            return RedirectToAction("Index", "Home");
        }
    }

    public enum FeedType
    {
        Rss,
        Atom,
    }
}