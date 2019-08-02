using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Vokabular.Core;
using Vokabular.MainService.Core.AutoMapperProfiles;
using Vokabular.MainService.Core.AutoMapperProfiles.Authentication;
using Vokabular.MainService.Core.AutoMapperProfiles.CardFile;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.Core.Managers.Authentication;
using Vokabular.MainService.Core.Managers.Fulltext;
using Vokabular.Shared.Container;

namespace Vokabular.MainService.Core
{
    public class MainServiceCoreContainerRegistration : IContainerInstaller
    {
        public void Install(IServiceCollection services)
        {
            services.AddScoped<AuthenticationManager>();
            services.AddScoped<AuthorizationManager>();
            services.AddScoped<BookHitSearchManager>();
            services.AddScoped<BookManager>();
            services.AddScoped<BookSearchManager>();
            services.AddScoped<CardFileManager>();
            services.AddScoped<CatalogValueManager>();
            services.AddScoped<CategoryManager>();
            services.AddScoped<CorpusSearchManager>();
            services.AddScoped<FavoriteManager>();
            services.AddScoped<FeedbackManager>();
            services.AddScoped<ForumSiteManager>();
            services.AddScoped<HeadwordSearchManager>();
            services.AddScoped<NamedResourceGroupManager>();
            services.AddScoped<NewsManager>();
            services.AddScoped<PermissionManager>();
            services.AddScoped<PersonManager>();
            services.AddScoped<ProjectContentManager>();
            services.AddScoped<ProjectInfoManager>();
            services.AddScoped<ProjectItemManager>();
            services.AddScoped<ProjectManager>();
            services.AddScoped<ProjectMetadataManager>();
            services.AddScoped<ProjectResourceManager>();
            services.AddScoped<TermManager>();
            services.AddScoped<UserDetailManager>();
            services.AddScoped<RoleManager>();
            services.AddScoped<UserManager>();

            services.AddScoped<CommunicationConfigurationProvider>();
            services.AddScoped<CommunicationProvider>();
            services.AddScoped<DefaultUserProvider>();
            services.AddScoped<FulltextStorageProvider>();
            services.AddScoped<IFulltextStorage, ExistDbStorage>();
            services.AddScoped<IFulltextStorage, ElasticSearchStorage>();

            services.AddSingleton<Profile, AuthUserProfile>();
            services.AddSingleton<Profile, RoleProfile>();
            services.AddSingleton<Profile, PermissionProfile>();

            services.AddSingleton<Profile, AudioProfile>();
            services.AddSingleton<Profile, BookProfile>();
            services.AddSingleton<Profile, BibliographicFormatProfile>();
            services.AddSingleton<Profile, CategoryProfile>();
            services.AddSingleton<Profile, ChapterProfile>();
            services.AddSingleton<Profile, EditionNoteProfile>();
            services.AddSingleton<Profile, ExternalRepositoryProfile>();
            services.AddSingleton<Profile, ExternalRepositoryTypeProfile>();
            services.AddSingleton<Profile, FavoriteLabelProfile>();
            services.AddSingleton<Profile, FavoriteProfile>();
            services.AddSingleton<Profile, FeedbackProfile>();
            services.AddSingleton<Profile, FilteringExpressionProfile>();
            services.AddSingleton<Profile, FilteringExpressionSetProfile>();
            services.AddSingleton<Profile, ForumProfile>();
            services.AddSingleton<Profile, HeadwordProfile>();
            services.AddSingleton<Profile, ImageProfile>();
            services.AddSingleton<Profile, KeywordProfile>();
            services.AddSingleton<Profile, LiteraryGenreProfile>();
            services.AddSingleton<Profile, LiteraryKindProfile>();
            services.AddSingleton<Profile, LiteraryOriginalProfile>();
            services.AddSingleton<Profile, MetadataProfile>();
            services.AddSingleton<Profile, NamedResourceGroupProfile>();
            services.AddSingleton<Profile, NewsProfile>();
            services.AddSingleton<Profile, OriginalAuthorProfile>();
            services.AddSingleton<Profile, PageProfile>();
            services.AddSingleton<Profile, ProjectProfile>();
            services.AddSingleton<Profile, ResourceProfile>();
            services.AddSingleton<Profile, ResponsiblePersonProfile>();
            services.AddSingleton<Profile, TermProfile>();
            services.AddSingleton<Profile, TextCommentProfile>();
            services.AddSingleton<Profile, TextProfile>();
            services.AddSingleton<Profile, TrackProfile>();
            services.AddSingleton<Profile, TransformationProfile>();
            services.AddSingleton<Profile, UserContactProfile>();
            services.AddSingleton<Profile, UserProfile>();

            services.AddSingleton<Profile, BucketContractProfile>();
            services.AddSingleton<Profile, BucketShortContractProfile>();
            services.AddSingleton<Profile, CardContractProfile>();
            services.AddSingleton<Profile, CardFileContractProfile>();
            services.AddSingleton<Profile, CardShortContractProfile>();

            new CoreContainerRegistration().Install(services);
        }
    }
}
