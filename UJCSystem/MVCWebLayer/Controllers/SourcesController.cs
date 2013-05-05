using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ujc.Naki.MVCWebLayer.Enums;
using Ujc.Naki.MVCWebLayer.Services;
using Ujc.Naki.MVCWebLayer.Services.Mocks;
using Ujc.Naki.MVCWebLayer.ViewModels;

namespace Ujc.Naki.MVCWebLayer.Controllers
{
    public sealed class SourcesController : Controller
    {
        private readonly SourcesMockProvider m_mockProvider = new SourcesMockProvider();

        [HttpGet]
        public ActionResult Detail(string id, string part)
        {
            var enumValue = SourceDetailPart.Info;
            try
            {
                enumValue = (SourceDetailPart)Enum.Parse(typeof(SourceDetailPart), part);
            }
            catch (Exception) {}

            switch (enumValue)
            {
                case SourceDetailPart.Info:
                    return View("Detail", m_mockProvider.GetDetail());
                case SourceDetailPart.Podminky:
                    return View("DetailPodminky");
                case SourceDetailPart.Zpracovani:
                    return View("DetailZpracovani");
                default:
                    return View("Detail", m_mockProvider.GetDetail());
            }
        }

        [HttpGet]
        public ActionResult DetailHledat(string id, string searchTerm)
        {
            return View("DetailSearch", new SearchSourceDetailViewModel
            {
                SearchTerm = string.Empty,
                ShowResults = !string.IsNullOrEmpty(searchTerm),
            });
        }

        [HttpGet]
        public ActionResult Prochazet(string id, int page)
        {
            /* WTF if (page == 1)
            {*/
                return View("Prochazet");
            /* }
            else
            {
                return View("Prochazet2");
            }*/
        }

        [HttpGet]
        public ActionResult Search(string searchTerm)
        {
            return View("Search", new SearchSourcesViewModel { 
                FoundSources = m_mockProvider.GetSearchResult(),
            });
        }

        [HttpGet]
        public ActionResult Listing(string alphabet, SourcesViewMode mode)
        {
            if (string.IsNullOrEmpty(alphabet))
            {
                alphabet = "A";
            }

            return View(new ListSourcesViewModel {
                ViewMode = mode,
                FoundSources = m_mockProvider.GetSources(alphabet, mode),
            });
        }
    }
}
