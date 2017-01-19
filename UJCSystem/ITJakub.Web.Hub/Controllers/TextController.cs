using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Core.Identity;
using ITJakub.Web.Hub.Core.Managers;
using ITJakub.Web.Hub.Core.Markdown;
using ITJakub.Web.Hub.Models;
using ITJakub.Web.Hub.Models.Requests;
using ITJakub.Web.Hub.Models.Type;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITJakub.Web.Hub.Controllers
{
    public class TextController : BaseController
    {
        private readonly StaticTextManager m_staticTextManager;
        private readonly IMarkdownToHtmlConverter m_markdownToHtmlConverter;

        public TextController(StaticTextManager staticTextManager, CommunicationProvider communicationProvider, IMarkdownToHtmlConverter markdownToHtmlConverter) : base(communicationProvider)
        {
            m_staticTextManager = staticTextManager;
            m_markdownToHtmlConverter = markdownToHtmlConverter;
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
                    result = m_markdownToHtmlConverter.ConvertToHtml(request.Text);
                    break;
                default:
                    result = request.Text;
                    break;
            }

            return Json(result);
        }
    }
}