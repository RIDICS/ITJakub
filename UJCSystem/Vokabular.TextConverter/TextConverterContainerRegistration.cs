using Microsoft.Extensions.DependencyInjection;
using Vokabular.TextConverter.Converters;

namespace Vokabular.TextConverter
{
    public static class TextConverterContainerRegistration 
    {
        public static void AddTextConverterServices(this IServiceCollection container)
        {
            container.AddScoped<IMarkdownToHtmlConverter, MarkdigMarkdownToHtmlConverter>();
            container.AddScoped<ITextConverter, TextConverter>();
        }
    }
}
