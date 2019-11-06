using System.Globalization;
using ITJakub.Web.Hub.Models;
using ITJakub.Web.Hub.Models.Type;
using Scalesoft.Localization.AspNetCore;
using Scalesoft.Localization.Core.Model;
using Scalesoft.Localization.Core.Util;
using Vokabular.TextConverter.Markdown;

namespace ITJakub.Web.Hub.Core.Managers
{
    public class StaticTextManager
    {
        private readonly IDynamicTextService m_dynamicTextService;
        private readonly ILocalizationService m_localizationService;
        private readonly IMarkdownToHtmlConverter m_markdownToHtmlConverter;

        public StaticTextManager(IDynamicTextService dynamicTextService, ILocalizationService localizationService, IMarkdownToHtmlConverter markdownToHtmlConverter)
        {
            m_dynamicTextService = dynamicTextService;
            m_localizationService = localizationService;
            m_markdownToHtmlConverter = markdownToHtmlConverter;
        }
        
        public EditStaticTextViewModel GetText(string name, string scope)
        {
            var staticText = m_dynamicTextService.GetDynamicText(name, scope);
            var currentCultureLabel = m_localizationService.GetRequestCulture().NativeName;

            if (staticText == null)
            {
                return new EditStaticTextViewModel
                {
                    Name = name,
                    Scope = scope,
                    IsRecordExists = false,
                    CultureNameLabel = currentCultureLabel,
                };
            }

            var staticTextViewModel = new EditStaticTextViewModel
            {
                Format = (StaticTextFormatType) staticText.Format,
                Name = staticText.Name,
                Scope = staticText.DictionaryScope,
                IsRecordExists = true,
                LastModificationAuthor = staticText.ModificationUser,
                LastModificationTime = staticText.ModificationTime,
                Text = staticText.Text,
                CultureNameLabel = currentCultureLabel,
            };
            
            return staticTextViewModel;
        }

        public StaticTextViewModel GetRenderedHtmlText(string name, string scope)
        {
            var text = m_localizationService.Translate(name, scope, LocTranslationSource.Database);

            var htmlText = m_markdownToHtmlConverter.ConvertToHtml(text);
            
            var viewModel = new StaticTextViewModel
            {
                Name = name,
                Text = htmlText,
                Scope = scope
            };

            return viewModel;
        }

        public ModificationUpdateViewModel SaveText(string name, string scope, string text, string culture, StaticTextFormatType format, string username)
        {
            var dynamicText = new DynamicText
            {
                Culture = culture,
                DictionaryScope = scope,
                Format = (short)format,
                ModificationUser = username,
                Name = name,
                Text = text
            };

            m_dynamicTextService.SaveDynamicText(dynamicText);

            var result = new ModificationUpdateViewModel
            {
                ModificationTime = dynamicText.ModificationTime.ToString(new CultureInfo(culture)),
                User = username
            };

            return result;
        }
    }
}