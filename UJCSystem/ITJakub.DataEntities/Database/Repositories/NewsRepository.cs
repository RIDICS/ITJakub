using System;
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

        [Transaction(TransactionMode.Requires)]
        public virtual IList<NewsSyndicationItem> GetWebNews(int start, int count)
        {
            using (var session = GetSession())
            {
                var items = session.QueryOver<NewsSyndicationItem>()
                    .Fetch(x=>x.User).Eager 
                    .Where(x=>x.ItemType == SyndicationItemType.Combined || x.ItemType == SyndicationItemType.Web)                   
                    .OrderBy(x => x.CreateDate).Desc.Skip(start).Take(count).List<NewsSyndicationItem>();

                return items;
            }
        }


        [Transaction(TransactionMode.Requires)]
        public virtual int GetWebNewsSyndicationItemCount()
        {
            using (var session = GetSession())
            {
                var items = session.QueryOver<NewsSyndicationItem>()
                    .Where(x => x.ItemType == SyndicationItemType.Combined || x.ItemType == SyndicationItemType.Web)
                    .RowCount();

                return items;
            }
        }
    }
}