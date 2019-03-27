using System.Linq;
using NHibernate.Criterion;
using Vokabular.DataEntities.Database.Daos;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.SelectResults;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.DataEntities.Database.Repositories
{
    public class ExternalRepositoryRepository : NHibernateDao
    {
        public ExternalRepositoryRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
      

        public virtual ExternalRepository GetExternalRepository(int externalRepositoryId)
        {
            return GetSession().QueryOver<ExternalRepository>()
                .Where(x => x.Id == externalRepositoryId)
                .SingleOrDefault();
        }

        public virtual ListWithTotalCountResult<ExternalRepository> GetExternalRepositoryList(int start, int count)
        {
            var query = GetSession().QueryOver<ExternalRepository>()
                .Fetch(x => x.CreatedByUser).Eager;

            var list = query.Skip(start)
                .Take(count)
                .Future();

            var totalCount = query.ToRowCountQuery()
                .FutureValue<int>();

            return new ListWithTotalCountResult<ExternalRepository>
            {
                List = list.ToList(),
                Count = totalCount.Value,
            };
        }
    }
}
