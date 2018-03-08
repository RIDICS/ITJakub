using System.Globalization;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Core.Managers;
using ITJakub.Web.Hub.Core.Markdown;
using ITJakub.Web.Hub.Models;
using ITJakub.Web.Hub.Models.Requests;
using ITJakub.Web.Hub.Models.Type;
using Localization.AspNetCore.Service;
using Localization.CoreLibrary.Manager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.Shared.Const;
using DynamicText = Localization.CoreLibrary.Entity.DynamicText;

namespace ITJakub.Web.Hub.Controllers
{
    public class TextController : BaseController
    {
        private readonly IAutoLocalizationManager m_localizationManager;

        private readonly StaticTextManager m_staticTextManager;
        private readonly IMarkdownToHtmlConverter m_markdownToHtmlConverter;

        public TextController(StaticTextManager staticTextManager, CommunicationProvider communicationProvider, IMarkdownToHtmlConverter markdownToHtmlConverter) : base(communicationProvider)
        {
            m_localizationManager = Localization.CoreLibrary.Localization.Translator;
            m_staticTextManager = staticTextManager;
            m_markdownToHtmlConverter = markdownToHtmlConverter;
        }

        [Authorize(Roles = CustomRole.CanEditStaticText)]
        public ActionResult Editor(string textName, string scope)
        {
            var viewModel = m_staticTextManager.GetText(textName, scope);
            return View("TextEditor", viewModel);
        }

        [Authorize(Roles = CustomRole.CanEditStaticText)]
        public ActionResult SaveText([FromBody] StaticTextViewModel viewModel)
        {
            var username = GetUserName();
            DynamicText dynamicText = new DynamicText()
            {
                Culture = viewModel.Name,
                DictionaryScope = viewModel.Scope,
                Format = (short)viewModel.Format,
                ModificationTime = viewModel.LastModificationTime,
                ModificationUser = viewModel.LastModificationAuthor,
                Name = viewModel.Name,
                Text = viewModel.Text
            };

            var modificationUpdate = m_staticTextManager.SaveText(viewModel.Name, viewModel.Scope, viewModel.Text, RequestCulture().Name ,viewModel.Format, username);
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

        private CultureInfo RequestCulture()
        {
            HttpRequest request = ControllerContext.HttpContext.Request;

            string cultureCookie = request.Cookies[ServiceBase.CultureCookieName];
            if (cultureCookie == null)
            {
                cultureCookie = m_localizationManager.DefaultCulture().Name;
            }

            return new CultureInfo(cultureCookie);
        }
    }
}