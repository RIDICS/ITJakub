using System.Collections.Generic;
using System.Transactions;
using Castle.Facilities.NHibernate;
using Castle.Transactions;
using ITJakub.DataEntities.Database.Daos;
using ITJakub.DataEntities.Database.Entities;

namespace ITJakub.DataEntities.Database.Repositories
{
    public class NewsRepository : NHibernateTransactionalDao<NewsSyndicationItem>
    {
        public NewsRepository(ISessionManager sessManager) : base(sessManager)
        {
        }

        [Transaction(TransactionScopeOption.Required)]
        public virtual IList<NewsSyndicationItem> GetWebNews(int start, int count)
        {
            using (var session = GetSession())
            {
                var items = session.QueryOver<NewsSyndicationItem>()
                    .Fetch(x => x.User).Eager
                    .Where(x => x.ItemType == SyndicationItemType.Combined || x.ItemType == SyndicationItemType.Web)
                    .OrderBy(x => x.CreateDate).Desc.Skip(start).Take(count).List<NewsSyndicationItem>();

                return items;
            }
        }

        [Transaction(TransactionScopeOption.Required)]
        public virtual IList<NewsSyndicationItem> GetMobileAppsNews(int start, int count)
        {
            using (var session = GetSession())
            {
                var items = session.QueryOver<NewsSyndicationItem>()
                    .Fetch(x => x.User).Eager
                    .Where(x => x.ItemType == SyndicationItemType.Combined || x.ItemType == SyndicationItemType.MobileApps)
                    .OrderBy(x => x.CreateDate).Desc.Skip(start).Take(count).List<NewsSyndicationItem>();

                return items;
            }
        }

        [Transaction(TransactionScopeOption.Required)]
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