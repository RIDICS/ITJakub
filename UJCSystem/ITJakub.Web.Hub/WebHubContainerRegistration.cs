﻿using AutoMapper;
using ITJakub.Web.Hub.Areas.Admin.AutomapperProfiles;
using ITJakub.Web.Hub.AutoMapperProfiles;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Core.Managers;
using ITJakub.Web.Hub.Core.Markdown;
using Microsoft.Extensions.DependencyInjection;
using Vokabular.Shared.Container;

namespace ITJakub.Web.Hub
{
    public class WebHubContainerRegistration : IContainerInstaller
    {
        public void Install(IServiceCollection services)
        {
            services.AddScoped<CommunicationProvider>();
            services.AddScoped<CommunicationConfigurationProvider>();
            services.AddScoped<StaticTextManager>();
            services.AddScoped<FeedbacksManager>();
            services.AddScoped<AuthenticationManager>();

            services.AddScoped<IMarkdownToHtmlConverter, MarkdigMarkdownToHtmlConverter>();

            // AutoMapper profiles
            services.AddSingleton<Profile, ConditionCriteriaDescriptionProfile>();
            services.AddSingleton<Profile, DatingCriteriaDescriptionProfile>();
            services.AddSingleton<Profile, DatingListCriteriaDescriptionProfile>();
            services.AddSingleton<Profile, FavoriteProfile>();
            services.AddSingleton<Profile, TokenDistanceCriteriaDescriptionProfile>();
            services.AddSingleton<Profile, TokenDistanceListCriteriaDescriptionProfile>();
            services.AddSingleton<Profile, WordCriteriaDescriptionProfile>();
            services.AddSingleton<Profile, WordListCriteriaDescriptionProfile>();

            // AutoMapper profiles - Admin area
            services.AddSingleton<Profile, LiteraryGenreProfile>();
            services.AddSingleton<Profile, LiteraryKindProfile>();
            services.AddSingleton<Profile, LiteraryOriginalProfile>();
            services.AddSingleton<Profile, CategoryProfile>();
            services.AddSingleton<Profile, ProjectProfile>();
            services.AddSingleton<Profile, ResourceProfile>();
            services.AddSingleton<Profile, ResponsibleTypeProfile>();
            services.AddSingleton<Profile, SnapshotProfile>();
            services.AddSingleton<Profile, UserProfile>();
        }
    }
}
