using System;
using System.Globalization;
using AutoMapper;
using ITJakub.Web.DataEntities.Database.Entities.Enums;
using ITJakub.Web.Hub.Core.Markdown;
using ITJakub.Web.Hub.Models;
using ITJakub.Web.Hub.Models.Type;
using Localization.AspNetCore.Service;
using DynamicText = Localization.CoreLibrary.Entity.DynamicText;

namespace ITJakub.Web.Hub.Core.Managers
{
    public class StaticTextManager
    {
        private readonly IDynamicText m_localizationService;
        private readonly IMarkdownToHtmlConverter m_markdownToHtmlConverter;

        public StaticTextManager(IDynamicText localizationService, IMarkdownToHtmlConverter markdownToHtmlConverter)
        {
            m_localizationService = localizationService;
            m_markdownToHtmlConverter = markdownToHtmlConverter;
        }
        
        public StaticTextViewModel GetText(string name, string scope)
        {
            var staticText = m_localizationService.GetDynamicText(name, scope);

            //var staticText = new GetStaticTextWork(m_staticTextRepository, name).Execute();
            if (staticText == null)
            {
                return new StaticTextViewModel
                {
                    Name = name,
                    IsRecordExists = false
                };
            }

            StaticTextViewModel staticTextViewModel = new StaticTextViewModel()
            {
                Format = (StaticTextFormatType)staticText.Format,
                IsRecordExists = true,
                LastModificationAuthor = staticText.ModificationUser,
                LastModificationTime = staticText.ModificationTime,
                Name = staticText.Name,
                Text = staticText.Text,
                Scope = staticText.DictionaryScope
            };

            return staticTextViewModel;
        }

        //TODO #Localization
        public StaticTextViewModel GetRenderedHtmlText(string name, string scope)
        {
            var staticText = m_localizationService.GetDynamicText(name, scope);

            //var staticTextEntity = new GetStaticTextWork(m_staticTextRepository, name).Execute();
            if (staticText == null)
            {
                return new StaticTextViewModel
                {
                    Name = name,
                    Format = StaticTextFormatType.Markdown,
                    IsRecordExists = false
                };
            }

            StaticTextViewModel viewModel = new StaticTextViewModel()
            {
                Format = (StaticTextFormatType)staticText.Format,
                IsRecordExists = true,
                LastModificationAuthor = staticText.ModificationUser,
                LastModificationTime = staticText.ModificationTime,
                Name = staticText.Name,
                Text = staticText.Text,
                Scope = staticText.DictionaryScope
            };

            StaticTextFormat typeFormat = (StaticTextFormat)staticText.Format;
            switch (typeFormat)
            {
                case StaticTextFormat.Markdown:
                    viewModel.Text = m_markdownToHtmlConverter.ConvertToHtml(staticText.Text);
                    viewModel.Format = StaticTextFormatType.Html;
                    break;
                case StaticTextFormat.PlainText:
                case StaticTextFormat.Html:
                    break;
            }

            return viewModel;
        }

        public ModificationUpdateViewModel SaveText(string name, string scope, string text, string culture, StaticTextFormatType format, string username)
        {
            //var formatType = Mapper.Map<StaticTextFormat>(format);
            DynamicText dynamicText = new DynamicText()
            {
                Culture = culture,
                DictionaryScope = scope,
                Format = (short)format,
                ModificationUser = username,
                Name = name,
                Text = text
            };

            m_localizationService.SaveDynamicText(dynamicText);

            ModificationUpdateViewModel result = new ModificationUpdateViewModel()
            {
                ModificationTime = dynamicText.ModificationTime.ToString(new CultureInfo(culture)),
                User = username
            };

            //var result = new SaveStaticTextWork(m_staticTextRepository, name, text, formatType, username).Execute();
            return result;
        }
    }
}