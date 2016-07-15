using System;
using System.Globalization;
using AutoMapper;
using ITJakub.Web.DataEntities.Database.Entities;
using ITJakub.Web.DataEntities.Database.Entities.Enums;
using ITJakub.Web.DataEntities.Database.Repositories;
using ITJakub.Web.Hub.Models;
using ITJakub.Web.Hub.Models.Type;
using MarkdownDeep;

namespace ITJakub.Web.Hub.Managers
{
    public class StaticTextManager
    {
        private readonly StaticTextRepository m_staticTextRepository;
        private readonly Markdown m_markdownDeep;

        public StaticTextManager(StaticTextRepository staticTextRepository)
        {
            m_staticTextRepository = staticTextRepository;
            m_markdownDeep = new Markdown
            {
                ExtraMode = true,
                SafeMode = false
            };
        }

        public StaticTextViewModel GetText(string name)
        {
            var staticText = m_staticTextRepository.GetStaticText(name);
            if (staticText == null)
            {
                return new StaticTextViewModel
                {
                    Name = name,
                    IsRecordExists = false
                };
            }

            var viewModel = Mapper.Map<StaticTextViewModel>(staticText);
            return viewModel;
        }

        public StaticTextViewModel GetRenderedHtmlText(string name)
        {
            var staticTextEntity = m_staticTextRepository.GetStaticText(name);
            if (staticTextEntity == null)
            {
                return new StaticTextViewModel
                {
                    Name = name,
                    Format = StaticTextFormatType.Markdown,
                    IsRecordExists = false
                };
            }

            var viewModel = Mapper.Map<StaticTextViewModel>(staticTextEntity);
            switch (staticTextEntity.Format)
            {
                case StaticTextFormat.Markdown:
                    viewModel.Text = m_markdownDeep.Transform(staticTextEntity.Text);
                    viewModel.Format = StaticTextFormatType.Html;
                    break;
                case StaticTextFormat.PlainText:
                case StaticTextFormat.Html:
                    break;
            }

            return viewModel;
        }

        public ModificationUpdateViewModel SaveText(string name, string text, StaticTextFormatType format, string username)
        {
            var now = DateTime.UtcNow;
            var staticTextEntity = m_staticTextRepository.GetStaticText(name);
            if (staticTextEntity == null)
            {
                staticTextEntity = new StaticText
                {
                    Name = name
                };
            }

            staticTextEntity.Text = text;
            staticTextEntity.Format = Mapper.Map<StaticTextFormat>(format);
            staticTextEntity.ModificationUser = username;
            staticTextEntity.ModificationTime = now;

            m_staticTextRepository.Save(staticTextEntity);

            return Mapper.Map<ModificationUpdateViewModel>(staticTextEntity);
        }
    }
}