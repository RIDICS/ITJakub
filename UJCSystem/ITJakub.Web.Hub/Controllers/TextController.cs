using System.Web.Mvc;
using ITJakub.Web.Hub.Managers;
using ITJakub.Web.Hub.Models;
using ITJakub.Web.Hub.Models.Type;
using MarkdownDeep;

namespace ITJakub.Web.Hub.Controllers
{
    public class TextController : Controller
    {
        private readonly StaticTextManager m_staticTextManager;
        private readonly Markdown m_markdownDeep;

        public TextController(StaticTextManager staticTextManager)
        {
            m_staticTextManager = staticTextManager;
            m_markdownDeep = new Markdown
            {
                ExtraMode = true,
                SafeMode = false
            };
        }

        public ActionResult Editor(string textName)
        {
            var viewModel = m_staticTextManager.GetText(textName);
            return View("TextEditor", viewModel);
        }

        public ActionResult SaveText(StaticTextViewModel viewModel)
        {
            m_staticTextManager.SaveText(viewModel.Name, viewModel.Text, viewModel.Format);
            return Json(new {});
        }

        public ActionResult RenderPreview(string text, StaticTextFormatType inputTextFormat)
        {
            string result;
            switch (inputTextFormat)
            {
                case StaticTextFormatType.Markdown:
                    result = m_markdownDeep.Transform(text);
                    break;
                default:
                    result = text;
                    break;
            }

            return Json(result);
        }
    }
}