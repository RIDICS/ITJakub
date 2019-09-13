using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.Repositories.BibliographyImport;
using Vokabular.DataEntities.Database.SearchCriteria;
using Vokabular.Shared.DataContracts.Search.QueryBuilder;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.DataEntities
{
    public class DataEntitiesContainerRegistration
    {
        public void Install(IServiceCollection services)
        {
            services.TryAddScoped<UnitOfWorkProvider>();

            services.AddScoped<BookViewRepository>();
            services.AddScoped<CatalogValueRepository>();
            services.AddScoped<CategoryRepository>();
            services.AddScoped<FavoritesRepository>();
            services.AddScoped<ExternalRepositoryRepository>();
            services.AddScoped<MetadataRepository>();
            services.AddScoped<FilteringExpressionSetRepository>();
            services.AddScoped<ImportHistoryRepository>();
            services.AddScoped<ImportedProjectMetadataRepository>();
            services.AddScoped<ImportedRecordMetadataRepository>();
            services.AddScoped<PermissionRepository>();
            services.AddScoped<PersonRepository>();
            services.AddScoped<PortalRepository>();
            services.AddScoped<ProjectRepository>();
            services.AddScoped<ResourceRepository>();
            services.AddScoped<SnapshotRepository>();
            services.AddScoped<UserRepository>();

            services.AddScoped<ICriteriaImplementationBase, AuthorCriteriaImplementation>();
            services.AddScoped<ICriteriaImplementationBase, AuthorizationCriteriaImplementation>();
            services.AddScoped<ICriteriaImplementationBase, CategoryCriteriaImplementation>();
            services.AddScoped<ICriteriaImplementationBase, DatingCriteriaImplementation>();
            services.AddScoped<ICriteriaImplementationBase, EditorCriteriaImplementation>();
            services.AddScoped<ICriteriaImplementationBase, HeadwordCriteriaImplementation>();
            services.AddScoped<ICriteriaImplementationBase, TermCriteriaImplementation>();
            services.AddScoped<ICriteriaImplementationBase, TitleCriteriaImplementation>();
        }
    }
}