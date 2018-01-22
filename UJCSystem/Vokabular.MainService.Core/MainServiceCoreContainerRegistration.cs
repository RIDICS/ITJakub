using AutoMapper;
using Vokabular.Core;
using Vokabular.MainService.Core.AutoMapperProfiles;
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
        public void Install(IIocContainer container)
        {
            container.AddPerWebRequest<AuthenticationManager>();
            container.AddPerWebRequest<AuthorizationManager>();
            container.AddPerWebRequest<BookHitSearchManager>();
            container.AddPerWebRequest<BookManager>();
            container.AddPerWebRequest<BookSearchManager>();
            container.AddPerWebRequest<CardFileManager>();
            container.AddPerWebRequest<CatalogValueManager>();
            container.AddPerWebRequest<CategoryManager>();
            container.AddPerWebRequest<CorpusSearchManager>();
            container.AddPerWebRequest<FavoriteManager>();
            container.AddPerWebRequest<FeedbackManager>();
            container.AddPerWebRequest<HeadwordSearchManager>();
            container.AddPerWebRequest<NamedResourceGroupManager>();
            container.AddPerWebRequest<NewsManager>();
            container.AddPerWebRequest<PermissionManager>();
            container.AddPerWebRequest<PersonManager>();
            container.AddPerWebRequest<ProjectContentManager>();
            container.AddPerWebRequest<ProjectInfoManager>();
            container.AddPerWebRequest<ProjectItemManager>();
            container.AddPerWebRequest<ProjectManager>();
            container.AddPerWebRequest<ProjectMetadataManager>();
            container.AddPerWebRequest<ProjectResourceManager>();
            container.AddPerWebRequest<TermManager>();
            container.AddPerWebRequest<UserGroupManager>();
            container.AddPerWebRequest<UserManager>();

            container.AddPerWebRequest<ICommunicationTokenGenerator, GuidCommunicationTokenGenerator>();
            container.AddPerWebRequest<ICommunicationTokenProvider, HttpHeaderCommunicationTokenProvider>();

            container.AddPerWebRequest<CommunicationConfigurationProvider>();
            container.AddPerWebRequest<CommunicationProvider>();
            container.AddPerWebRequest<DefaultUserProvider>();
            container.AddPerWebRequest<FulltextStorageProvider>();
            container.AddPerWebRequest<IFulltextStorage, ExistDbStorage>();
            container.AddPerWebRequest<IFulltextStorage, ElasticSearchStorage>();

            container.AddSingleton<Profile, AudioProfile>();
            container.AddSingleton<Profile, BookProfile>();
            container.AddSingleton<Profile, CategoryProfile>();
            container.AddSingleton<Profile, ChapterProfile>();
            container.AddSingleton<Profile, EditionNoteProfile>();
            container.AddSingleton<Profile, FavoriteLabelProfile>();
            container.AddSingleton<Profile, FavoriteProfile>();
            container.AddSingleton<Profile, FeedbackProfile>();
            container.AddSingleton<Profile, HeadwordProfile>();
            container.AddSingleton<Profile, ImageProfile>();
            container.AddSingleton<Profile, KeywordProfile>();
            container.AddSingleton<Profile, LiteraryGenreProfile>();
            container.AddSingleton<Profile, LiteraryKindProfile>();
            container.AddSingleton<Profile, LiteraryOriginalProfile>();
            container.AddSingleton<Profile, MetadataProfile>();
            container.AddSingleton<Profile, NamedResourceGroupProfile>();
            container.AddSingleton<Profile, NewsProfile>();
            container.AddSingleton<Profile, OriginalAuthorProfile>();
            container.AddSingleton<Profile, PageProfile>();
            container.AddSingleton<Profile, ProjectProfile>();
            container.AddSingleton<Profile, ResourceProfile>();
            container.AddSingleton<Profile, ResponsiblePersonProfile>();
            container.AddSingleton<Profile, SpecialPermissionProfile>();
            container.AddSingleton<Profile, TermProfile>();
            container.AddSingleton<Profile, TextCommentProfile>();
            container.AddSingleton<Profile, TextProfile>();
            container.AddSingleton<Profile, TrackProfile>();
            container.AddSingleton<Profile, TransformationProfile>();
            container.AddSingleton<Profile, UserGroupProfile>();
            container.AddSingleton<Profile, UserProfile>();

            container.AddSingleton<Profile, BucketContractProfile>();
            container.AddSingleton<Profile, BucketShortContractProfile>();
            container.AddSingleton<Profile, CardContractProfile>();
            container.AddSingleton<Profile, CardFileContractProfile>();
            container.AddSingleton<Profile, CardShortContractProfile>();

            container.Install<CoreContainerRegistration>();
        }
    }
}
