using Microsoft.Extensions.DependencyInjection;
using Vokabular.Marc21ProjectParser.ControlFieldProcessors;
using Vokabular.Marc21ProjectParser.DataFieldProcessors;
using Vokabular.Marc21ProjectParser.Filtering;
using Vokabular.ProjectParsing;
using Vokabular.ProjectParsing.Parsers;

namespace Vokabular.Marc21ProjectParser
{
    public static class Marc21ProjectParsingContainerRegistration
    {
        public static void AddMarc21ProjectParsingServices(this IServiceCollection services)
        {
            services.AddSingleton<IProjectParser, Marc21ProjectParser>();

            services.AddSingleton<IProjectFilter, Marc21ProjectFilter>();

            services.AddSingleton<IControlFieldProcessor, OriginalResourceUrlProcessor>();

            services.AddSingleton<IDataFieldProcessor, AuthorProcessor>();
            services.AddSingleton<IDataFieldProcessor, EditionNoteProcessor>();
            services.AddSingleton<IDataFieldProcessor, GenreProcessor>();
            services.AddSingleton<IDataFieldProcessor, KeywordProcessor>();
            services.AddSingleton<IDataFieldProcessor, OriginDateProcessor>();
            services.AddSingleton<IDataFieldProcessor, ProjectNameProcessor>();
            services.AddSingleton<IDataFieldProcessor, PublishInfoProcessor>();
            services.AddSingleton<IDataFieldProcessor, PublishInfo2Processor>();
        }
    }
}
