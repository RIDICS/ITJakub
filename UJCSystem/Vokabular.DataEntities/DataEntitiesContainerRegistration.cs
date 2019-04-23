using Microsoft.Extensions.DependencyInjection;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.SearchCriteria;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.Shared.Container;
using Vokabular.Shared.DataContracts.Search.QueryBuilder;

namespace Vokabular.DataEntities
{
    public static class DataEntitiesContainerRegistration
    {
        public static void AddDataEntitiesServices(this IServiceCollection container)
        {
            container.AddScoped<IUnitOfWork, UnitOfWork>();

            container.AddScoped<BookRepository>();
            container.AddScoped<CatalogValueRepository>();
            container.AddScoped<CategoryRepository>();
            container.AddScoped<ExternalRepositoryRepository>();
            container.AddScoped<FavoritesRepository>();
            container.AddScoped<FilteringExpressionSetRepository>();
            container.AddScoped<ImportHistoryRepository>();
            container.AddScoped<ImportedProjectMetadataRepository>();
            container.AddScoped<ImportedRecordMetadataRepository>();
            container.AddScoped<MetadataRepository>();
            container.AddScoped<PermissionRepository>();
            container.AddScoped<PersonRepository>();
            container.AddScoped<PortalRepository>();
            container.AddScoped<ProjectRepository>();
            container.AddScoped<ResourceRepository>();
            container.AddScoped<UserRepository>();

            container.AddScoped<ICriteriaImplementationBase, AuthorCriteriaImplementation>();
            container.AddScoped<ICriteriaImplementationBase, AuthorizationCriteriaImplementation>();
            container.AddScoped<ICriteriaImplementationBase, CategoryCriteriaImplementation>();
            container.AddScoped<ICriteriaImplementationBase, DatingCriteriaImplementation>();
            container.AddScoped<ICriteriaImplementationBase, EditorCriteriaImplementation>();
            container.AddScoped<ICriteriaImplementationBase, HeadwordCriteriaImplementation>();
            container.AddScoped<ICriteriaImplementationBase, TermCriteriaImplementation>();
            container.AddScoped<ICriteriaImplementationBase, TitleCriteriaImplementation>();
        }
    }
}
