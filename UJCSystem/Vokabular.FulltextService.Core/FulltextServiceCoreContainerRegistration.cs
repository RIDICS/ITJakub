using Microsoft.Extensions.DependencyInjection;
using Vokabular.FulltextService.Core.Communication;
using Vokabular.FulltextService.Core.Helpers;
using Vokabular.FulltextService.Core.Helpers.Converters;
using Vokabular.FulltextService.Core.Helpers.Hml;
using Vokabular.FulltextService.Core.Helpers.Markdown;
using Vokabular.FulltextService.Core.Helpers.Validators;
using Vokabular.FulltextService.Core.Managers;
using Vokabular.Shared.Container;

namespace Vokabular.FulltextService.Core
{
    public class FulltextServiceCoreContainerRegistration : IContainerInstaller
    {
        public void Install(IServiceCollection services)
        {
            services.AddScoped<CommunicationConfigurationProvider>();
            services.AddScoped<CommunicationProvider>();
            services.AddScoped<TextResourceManager>();
            services.AddScoped<SnapshotResourceManager>();
            services.AddScoped<SearchManager>();
            services.AddScoped<SearchResultProcessor>();
            services.AddScoped<QueriesBuilder>();
            services.AddScoped<SnapshotResourceBuilder>();

            services.AddScoped<IMarkdownToHtmlConverter, MarkdigMarkdownToHtmlConverter>();
            services.AddScoped<IHtmlToPlainTextConverter, HtmlToPlainTextConverter>();
            services.AddScoped<IMarkdownToPlainTextConverter, MarkdownToPlainTextConverter>();
            services.AddScoped<ITextConverter, TextConverter>();
            services.AddScoped<ITextValidator, TextValidator>();
            services.AddScoped<IPageWithHtmlTagsCreator, PageWithHtmlTagsCreator>();
        }
    }
}
