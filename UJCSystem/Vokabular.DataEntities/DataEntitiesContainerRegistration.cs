using Microsoft.Extensions.DependencyInjection;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.SearchCriteria;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.Shared.DataContracts.Search.QueryBuilder;

namespace Vokabular.DataEntities
{
    public class DataEntitiesContainerRegistration
    {
        public void Install(IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<BookRepository>();
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