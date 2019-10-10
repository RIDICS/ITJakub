using Microsoft.Extensions.DependencyInjection;
using Vokabular.TextConverter.Html;
using Vokabular.TextConverter.Markdown;
using Vokabular.TextConverter.Markdown.Extensions;
using Vokabular.TextConverter.Markdown.Extensions.CommentMark;

namespace Vokabular.TextConverter
{
    public static class TextConverterContainerRegistration 
    {
        public static void AddTextConverterServices(this IServiceCollection services)
        {
            services.AddScoped<IMarkdownToHtmlConverter, MarkdigMarkdownToHtmlConverter>();
            services.AddScoped<IHtmlToPlainTextConverter, HtmlToPlainTextConverter>();
            services.AddScoped<IMarkdownToPlainTextConverter, MarkdownToPlainTextConverter>();

            services.AddScoped<MarkdownCommentAnalyzer>();
            services.AddScoped<MarkdownHeadingAnalyzer>();
        }
    }
}
