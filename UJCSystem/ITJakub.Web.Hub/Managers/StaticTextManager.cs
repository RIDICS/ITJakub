using System;
using AutoMapper;
using ITJakub.Web.DataEntities.Database.Entities;
using ITJakub.Web.DataEntities.Database.Entities.Enums;
using ITJakub.Web.DataEntities.Database.Repositories;
using ITJakub.Web.Hub.Models;
using ITJakub.Web.Hub.Models.Type;
using Markdig;

namespace ITJakub.Web.Hub.Managers
{
    public class StaticTextManager
    {
        private readonly StaticTextRepository m_staticTextRepository;

        public StaticTextManager(StaticTextRepository staticTextRepository)
        {
            m_staticTextRepository = staticTextRepository;
        }

        public string MarkdownToHtml(string markdown)
        {
            var pipeline = new MarkdownPipelineBuilder()
                .UseAbbreviations()
                .UseCitations()
                .UseCustomContainers()
                .UseDefinitionLists()
                .UseEmphasisExtras()
                .UseFigures()
                .UseFooters()
                .UseFootnotes()
                .UseGridTables()
                .UseMediaLinks()
                .UsePipeTables()
                .UseListExtras()
                .UseTaskLists()
                .Build();
            var result = Markdown.ToHtml(markdown, pipeline);
            return result;
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
                    viewModel.Text = MarkdownToHtml(staticTextEntity.Text);
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