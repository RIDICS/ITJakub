using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.SearchCriteria;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.Shared.Container;
using Vokabular.Shared.DataContracts.Search.QueryBuilder;

namespace Vokabular.DataEntities
{
    public class DataEntitiesContainerRegistration : IContainerInstaller
    {
        public void Install(IIocContainer container)
        {
            container.AddPerWebRequest<IUnitOfWork, UnitOfWork>();

            container.AddPerWebRequest<BookRepository>();
            container.AddPerWebRequest<CategoryRepository>();
            container.AddPerWebRequest<MetadataRepository>();
            container.AddPerWebRequest<PersonRepository>();
            container.AddPerWebRequest<ProjectRepository>();
            container.AddPerWebRequest<ResourceRepository>();
            container.AddPerWebRequest<UserRepository>();

            container.AddPerWebRequest<ICriteriaImplementationBase, AuthorCriteriaImplementation>();
            container.AddPerWebRequest<ICriteriaImplementationBase, AuthorizationCriteriaImplementation>();
            container.AddPerWebRequest<ICriteriaImplementationBase, CategoryCriteriaImplementation>();
            container.AddPerWebRequest<ICriteriaImplementationBase, DatingCriteriaImplementation>();
            container.AddPerWebRequest<ICriteriaImplementationBase, EditorCriteriaImplementation>();
            container.AddPerWebRequest<ICriteriaImplementationBase, HeadwordCriteriaImplementation>();
            container.AddPerWebRequest<ICriteriaImplementationBase, TermCriteriaImplementation>();
            container.AddPerWebRequest<ICriteriaImplementationBase, TitleCriteriaImplementation>();
        }
    }
}
