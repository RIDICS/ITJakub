using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.DataEntities.Database.Daos;
using ITJakub.DataEntities.Database.Entities;
using NHibernate;

namespace ITJakub.DataEntities.Database.Repositories
{
    [Transactional]
    public class CategoryRepository : NHibernateTransactionalDao
    {
        public CategoryRepository(ISessionManager sessManager)
            : base(sessManager)
        {
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void SaveOrUpdate(Category category)
        {
            using (ISession session = GetSession())
            {
                var tmpCategory = FindByXmlId(category.XmlId);
                if (tmpCategory != null)
                {
                    tmpCategory.Description = category.Description;
                    session.SaveOrUpdate(tmpCategory);
                }
                else
                {
                    session.SaveOrUpdate(category);
                }
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual Category FindByXmlId(string xmlId)
        {
            using (ISession session = GetSession())
            {
                return session.QueryOver<Category>()
                    .Where(category => category.XmlId == xmlId)
                    .SingleOrDefault<Category>();
            }
        }
    }
}