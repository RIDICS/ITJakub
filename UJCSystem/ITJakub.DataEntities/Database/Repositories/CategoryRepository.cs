using System.Collections.Generic;
using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.DataEntities.Database.Daos;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Entities.Enums;
using ITJakub.DataEntities.Database.Exceptions;
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

        [Transaction(TransactionMode.Requires)]
        public virtual void SetBookTypeToRootCategoryIfNotKnown(BookType bookType, Category rootCategory)
        {
            using (ISession session = GetSession())
            {
                var rootCategoryWithBookType =
                    session.QueryOver<Category>()
                        .Where(cat => cat.ParentCategory == null && cat.BookType == bookType)
                        .SingleOrDefault<Category>();

                if (rootCategoryWithBookType != null && rootCategoryWithBookType.Id != rootCategory.Id)
                {
                    throw new BookTypeIsAlreadyAssociatedWithAnotherCategoryException(bookType.Id, rootCategoryWithBookType.Id);
                }

                rootCategory.BookType = bookType;
                session.Update(rootCategory);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<Category> FindCategoriesByBookType(BookTypeEnum type)
        {
            Category categoryAlias = null;
            BookType bookTypeAlias = null;

            using (var session = GetSession())
            {
                return
                    session.QueryOver(() => categoryAlias)
                        .JoinAlias(x => x.BookType, () => bookTypeAlias)
                        .Where(() => bookTypeAlias.Type == type)
                        .List<Category>();
            }
        }
    }
}