using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Core.Managers;
using ITJakub.Web.Hub.Models;
using ITJakub.Web.Hub.Models.Requests;
using ITJakub.Web.Hub.Models.Type;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scalesoft.Localization.AspNetCore;
using Vokabular.Shared.Const;
using Vokabular.TextConverter.Markdown;

namespace ITJakub.Web.Hub.Controllers
{
    public class TextController : BaseController
    {
        private readonly ILocalizationService m_localizationService;
        private readonly StaticTextManager m_staticTextManager;
        private readonly IMarkdownToHtmlConverter m_markdownToHtmlConverter;

        public TextController(ILocalizationService localizationService, StaticTextManager staticTextManager, CommunicationProvider communicationProvider, IMarkdownToHtmlConverter markdownToHtmlConverter) : base(communicationProvider)
        {
            m_localizationService = localizationService;
            m_staticTextManager = staticTextManager;
            m_markdownToHtmlConverter = markdownToHtmlConverter;
        }

        [Authorize(VokabularPermissionNames.EditStaticText)]
        public ActionResult Editor(string textName, string scope)
        {
            var viewModel = m_staticTextManager.GetText(textName, scope);
            return View("TextEditor", viewModel);
        }

        [Authorize(VokabularPermissionNames.EditStaticText)]
        public ActionResult SaveText([FromBody] EditStaticTextViewModel viewModel)
        {
            var username = GetUserName();
            var culture = m_localizationService.GetRequestCulture();

            var modificationUpdate = m_staticTextManager.SaveText(viewModel.Name, viewModel.Scope, viewModel.Text, culture.Name, viewModel.Format, username);
            return Json(modificationUpdate);
        }

        [Authorize(VokabularPermissionNames.EditStaticText)]
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