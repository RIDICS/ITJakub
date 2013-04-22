using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ujc.Naki.MVCWebLayer.ViewModels;

namespace Ujc.Naki.MVCWebLayer.Controllers
{
    public class IndexController : Controller
    {
        public ActionResult Index()
        {
            return View(new SearchViewModel());
        }

    }
}
