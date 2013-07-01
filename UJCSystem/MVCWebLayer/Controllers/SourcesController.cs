using System;
using System.Web.Mvc;
using ITJakub.MVCWebLayer.Enums;
using ITJakub.MVCWebLayer.Services;
using ITJakub.MVCWebLayer.Services.Mocks;
using ITJakub.MVCWebLayer.ViewModels;

namespace ITJakub.MVCWebLayer.Controllers
{
    public sealed class SourcesController : Controller
    {
        private readonly ISourcesProvider m_provider = new MockSourcesProvider();

        [HttpGet]
        public ActionResult Detail(string id, string part)
        {
            var enumValue =  SourceDetailPart.Info;
            try
            {
                enumValue = (SourceDetailPart)Enum.Parse(typeof(SourceDetailPart), part);
            }
            catch (Exception) {}

            switch (enumValue)
            {
                case SourceDetailPart.Info:
                    return View("Detail", m_provider.GetDetail(id));
                case SourceDetailPart.Podminky:
                    return View("DetailPodminky");
                case SourceDetailPart.Zpracovani:
                    return View("DetailZpracovani");
                default:
                    return View("Detail", m_provider.GetDetail(id));
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
                return View("Prochazet");
        }

        [HttpGet]
        public ActionResult Search(string searchTerm)
        {
            return View("Search", new SearchSourcesViewModel { 
                FoundSources = m_provider.GetSearchResult(),
            });
        }

        [HttpGet]
        public ActionResult Listing(string alphabet, string mode)
        {
            alphabet = alphabet.ToUpper();
            if (string.IsNullOrEmpty(alphabet))
            {
                alphabet = "A";
            }

            return View(new ListSourcesViewModel {
                ViewMode = SourcesViewModeConverter.FromUrlParam(mode),
                FoundSources = m_provider.GetSources(alphabet, SourcesViewModeConverter.FromUrlParam(mode)),
            });
        }


        

        
    }
}
