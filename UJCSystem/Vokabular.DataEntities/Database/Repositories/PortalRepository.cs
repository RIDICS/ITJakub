using System.Linq;
using Vokabular.DataEntities.Database.Daos;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Entities.SelectResults;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.DataEntities.Database.Repositories
{
    public class PortalRepository : NHibernateDao
    {
        public PortalRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public virtual ListWithTotalCountResult<NewsSyndicationItem> GetNewsSyndicationItems(int start, int count, SyndicationItemType? type)
        {
            var query = GetSession().QueryOver<NewsSyndicationItem>()
                .Fetch(x => x.User).Eager
                .OrderBy(x => x.CreateTime).Desc;

            if (type != null)
            {
                query = query.Where(x => x.ItemType == type || x.ItemType == SyndicationItemType.Combined);
            }

            var list = query.Skip(start)
                .Take(count)
                .Future();

            var totalCount = query.ToRowCountQuery()
                .FutureValue<int>();

            return new ListWithTotalCountResult<NewsSyndicationItem>
            {
                List = list.ToList(),
                Count = totalCount.Value,
            };
        }
    }
}
