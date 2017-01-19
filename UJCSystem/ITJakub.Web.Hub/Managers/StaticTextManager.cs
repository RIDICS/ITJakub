using AutoMapper;
using ITJakub.Web.DataEntities.Database.Entities.Enums;
using ITJakub.Web.DataEntities.Database.Repositories;
using ITJakub.Web.Hub.Managers.Markdown;
using ITJakub.Web.Hub.Managers.Work;
using ITJakub.Web.Hub.Models;
using ITJakub.Web.Hub.Models.Type;

namespace ITJakub.Web.Hub.Managers
{
    public class StaticTextManager
    {
        private readonly StaticTextRepository m_staticTextRepository;
        private readonly IMarkdownToHtmlConverter m_markdownToHtmlConverter;

        public StaticTextManager(StaticTextRepository staticTextRepository, IMarkdownToHtmlConverter markdownToHtmlConverter)
        {
            m_staticTextRepository = staticTextRepository;
            m_markdownToHtmlConverter = markdownToHtmlConverter;
        }
        
        public StaticTextViewModel GetText(string name)
        {
            var staticText = new GetStaticTextWork(m_staticTextRepository, name).Execute();
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
            var staticTextEntity = new GetStaticTextWork(m_staticTextRepository, name).Execute();
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
                    viewModel.Text = m_markdownToHtmlConverter.ConvertToHtml(staticTextEntity.Text);
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
            var formatType = Mapper.Map<StaticTextFormat>(format);
            var result = new SaveStaticTextWork(m_staticTextRepository, name, text, formatType, username).Execute();
            return result;
        }
    }
}