using ITJakub.Web.Hub.Identity;
using ITJakub.Web.Hub.Managers;
using ITJakub.Web.Hub.Models;
using ITJakub.Web.Hub.Models.Requests;
using ITJakub.Web.Hub.Models.Type;
using Markdig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITJakub.Web.Hub.Controllers
{
    public class TextController : BaseController
    {
        private readonly StaticTextManager m_staticTextManager;

        public TextController(StaticTextManager staticTextManager, CommunicationProvider communicationProvider) : base(communicationProvider)
        {
            m_staticTextManager = staticTextManager;
        }

        [Authorize(Roles = CustomRole.CanEditStaticText)]
        public ActionResult Editor(string textName)
        {
            var viewModel = m_staticTextManager.GetText(textName);
            return View("TextEditor", viewModel);
        }

        [Authorize(Roles = CustomRole.CanEditStaticText)]
        public ActionResult SaveText([FromBody] StaticTextViewModel viewModel)
        {
            var username = GetUserName();
            var modificationUpdate = m_staticTextManager.SaveText(viewModel.Name, viewModel.Text, viewModel.Format, username);
            return Json(modificationUpdate);
        }

        [Authorize(Roles = CustomRole.CanEditStaticText)]
        public ActionResult RenderPreview([FromBody] RenderTextPreviewRequest request)
        {
            string result;
            switch (request.InputTextFormat)
            {
                case StaticTextFormatType.Markdown:
                    result = m_staticTextManager.MarkdownToHtml(request.Text);
                    break;
                default:
                    result = request.Text;
                    break;
            }

            return Json(result);
        }
    }
}