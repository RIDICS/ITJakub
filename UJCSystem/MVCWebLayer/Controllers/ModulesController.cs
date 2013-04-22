using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ujc.Naki.MVCWebLayer.ViewModels;
using Ujc.Naki.MVCWebLayer.Services;
using Ujc.Naki.MVCWebLayer.Enums;

namespace Ujc.Naki.MVCWebLayer.Controllers
{
    public class ModulesController : Controller
    {
        [HttpGet]
        public virtual ActionResult Index(string id)
        {
            return View();
        }
    }
}
