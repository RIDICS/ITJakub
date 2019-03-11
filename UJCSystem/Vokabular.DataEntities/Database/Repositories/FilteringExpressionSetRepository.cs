using System.Collections.Generic;
using Vokabular.DataEntities.Database.Daos;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.DataEntities.Database.Repositories
{
    public class FilteringExpressionSetRepository : NHibernateDao
    {
        public FilteringExpressionSetRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }


        public IList<FilteringExpression> GetFilteringExpressionsByExternalRepository(int externalRepositoryId)
        {
            FilteringExpressionSet setAlias = null;
            ExternalRepository repositoryAlias = null;

            return GetSession().QueryOver<FilteringExpression>()
                .JoinAlias(x => x.FilteringExpressionSet, () => setAlias)
                .JoinAlias(() => setAlias.ExternalRepositories, () => repositoryAlias)
                .Where(() => repositoryAlias.Id == externalRepositoryId)
                .List();
        }
    }
}