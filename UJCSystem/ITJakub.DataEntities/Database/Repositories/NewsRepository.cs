using System.Collections.Generic;
using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.DataEntities.Database.Daos;
using ITJakub.DataEntities.Database.Entities;

namespace ITJakub.DataEntities.Database.Repositories
{
    [Transactional]
    public class NewsRepository:NHibernateTransactionalDao<NewsSyndicationItem>
    {
        public NewsRepository(ISessionManager sessManager) : base(sessManager)
        {
        }

        public virtual IList<NewsSyndicationItem> GetNews(int start, int count)
        {
            using (var session = GetSession())
            {
                var items = session.QueryOver<NewsSyndicationItem>()
                    .Fetch(x=>x.User).Eager
                    .OrderBy(x => x.CreateDate).Desc.Skip(start).Take(count).List<NewsSyndicationItem>();

                return items;
            }
        }
    }
}