using System.Web.Mvc;
using ITJakub.Web.Hub.Models.Type;
using MarkdownDeep;

namespace ITJakub.Web.Hub.Controllers
{
    public class TextController : Controller
    {
        private readonly Markdown m_markdownDeep;

        public TextController()
        {
            m_markdownDeep = new Markdown
            {
                ExtraMode = true,
                SafeMode = false
            };
        }

        public ActionResult Editor()
        {
            return View("TextEditor");
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