﻿using Microsoft.Extensions.DependencyInjection;
using Vokabular.Marc21ProjectParser.ControlFieldProcessors;
using Vokabular.Marc21ProjectParser.DataFieldProcessors;
using Vokabular.ProjectParsing;
using Vokabular.ProjectParsing.Parsers;

namespace Vokabular.Marc21ProjectParser
{
    public static class Marc21ProjectParsingContainerRegistration
    {
        public static void AddMarc21ProjectParsingServices(this IServiceCollection services)
        {
            services.AddSingleton<IParser, Marc21Parser>();

            services.AddSingleton<IControlFieldProcessor, OriginalResourceUrlProcessor>();

            services.AddSingleton<IDataFieldProcessor, AuthorProcessor>();
            services.AddSingleton<IDataFieldProcessor, KeywordProcessor>();
            services.AddSingleton<IDataFieldProcessor, OriginDateProcessor>();
            services.AddSingleton<IDataFieldProcessor, ProjectNameProcessor>();
            services.AddSingleton<IDataFieldProcessor, PublishInfoProcessor>();
            services.AddSingleton<IDataFieldProcessor, PublishInfo2Processor>();
        }
    }
}
