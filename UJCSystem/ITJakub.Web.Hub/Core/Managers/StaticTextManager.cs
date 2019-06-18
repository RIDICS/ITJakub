using System;
using System.Globalization;
using ITJakub.Web.Hub.Core.Markdown;
using ITJakub.Web.Hub.Models;
using ITJakub.Web.Hub.Models.Type;
using Scalesoft.Localization.AspNetCore;
using Scalesoft.Localization.Core.Model;

namespace ITJakub.Web.Hub.Core.Managers
{
    public class StaticTextManager
    {
        private readonly IDynamicTextService m_dynamicTextService;
        private readonly IMarkdownToHtmlConverter m_markdownToHtmlConverter;

        public StaticTextManager(IDynamicTextService dynamicTextService, IMarkdownToHtmlConverter markdownToHtmlConverter)
        {
            m_dynamicTextService = dynamicTextService;
            m_markdownToHtmlConverter = markdownToHtmlConverter;
        }
        
        public StaticTextViewModel GetText(string name, string scope)
        {
            var staticText = m_dynamicTextService.GetDynamicText(name, scope);           

            if (staticText == null)
            {
                return new StaticTextViewModel
                {
                    Name = name,
                    IsRecordExists = false
                };
            }

            var staticTextViewModel = new StaticTextViewModel
            {
                Format = (StaticTextFormatType) staticText.Format,
                Name = staticText.Name,
                Scope = staticText.DictionaryScope
            };

            if (staticText.FallBack)
            {
                staticTextViewModel.IsRecordExists = false;
                staticTextViewModel.LastModificationTime = new DateTime();
                staticTextViewModel.Text = string.Empty;
                staticTextViewModel.LastModificationAuthor = string.Empty;
            }
            else
            {
                staticTextViewModel.IsRecordExists = true;
                staticTextViewModel.LastModificationAuthor = staticText.ModificationUser;
                staticTextViewModel.LastModificationTime = staticText.ModificationTime;
                staticTextViewModel.Text = staticText.Text;
            }

            return staticTextViewModel;
        }

        //TODO #Localization
        public StaticTextViewModel GetRenderedHtmlText(string name, string scope)
        {
            var staticText = m_dynamicTextService.GetDynamicText(name, scope);
            if (staticText == null)
            {
                return new StaticTextViewModel
                {
                    Name = name,
                    Format = StaticTextFormatType.Markdown,
                    IsRecordExists = false
                };
            }

            var viewModel = new StaticTextViewModel
            {
                Format = (StaticTextFormatType)staticText.Format,
                IsRecordExists = true,
                LastModificationAuthor = staticText.ModificationUser,
                LastModificationTime = staticText.ModificationTime,
                Name = staticText.Name,
                Text = staticText.Text,
                Scope = staticText.DictionaryScope
            };

            switch (viewModel.Format)
            {
                case StaticTextFormatType.Markdown:
                    viewModel.Text = m_markdownToHtmlConverter.ConvertToHtml(staticText.Text);
                    viewModel.Format = StaticTextFormatType.Html;
                    break;
                case StaticTextFormatType.PlainText:
                case StaticTextFormatType.Html:
                    break;
            }

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