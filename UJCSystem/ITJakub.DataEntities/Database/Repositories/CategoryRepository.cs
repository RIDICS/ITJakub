using System.Collections.Generic;
using System.Linq;
using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.DataEntities.Database.Daos;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Entities.Enums;
using ITJakub.DataEntities.Database.Exceptions;
using NHibernate.Criterion;

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
            using (var session = GetSession())
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
            using (var session = GetSession())
            {
                return session.QueryOver<Category>()
                    .Where(category => category.XmlId == xmlId)
                    .SingleOrDefault<Category>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void SetBookTypeToRootCategoryIfNotKnown(BookType bookType, Category rootCategory)
        {
            using (var session = GetSession())
            {
                var rootCategoryWithBookType =
                    session.QueryOver<Category>()
                        .Where(cat => cat.ParentCategory == null && cat.BookType == bookType)
                        .SingleOrDefault<Category>();

                if (rootCategoryWithBookType != null && rootCategoryWithBookType.Id != rootCategory.Id)
                {
                    throw new BookTypeIsAlreadyAssociatedWithAnotherCategoryException(bookType.Id,
                        rootCategoryWithBookType.Id);
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
                var rootCategory = session.QueryOver(() => categoryAlias)
                    .JoinAlias(x => x.BookType, () => bookTypeAlias)
                    .Where(() => bookTypeAlias.Type == type)
                    .SingleOrDefault<Category>();

                IList<int> parentCategoriesIds;
                var resultCategories = new List<Category>();
                IList<Category> childCategories = new List<Category> {rootCategory};
                
                while (childCategories != null && childCategories.Count() != 0)
                {
                    resultCategories.AddRange(childCategories);

                    parentCategoriesIds = childCategories.Select(childCategory => childCategory.Id).ToList();
                    childCategories = session.QueryOver(() => categoryAlias)
                        .Where(() => categoryAlias.ParentCategory.Id.IsIn(parentCategoriesIds.ToArray()))
                        .List<Category>();
                }

                return resultCategories;
            }
        }
    }
}