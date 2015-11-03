using System.Collections.Generic;
using System.Linq;
using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.DataEntities.Database.Daos;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Entities.Enums;
using ITJakub.DataEntities.Database.Exceptions;
using NHibernate.Criterion;
using NHibernate.Mapping;
using NHibernate.Transform;

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
                        .Where(cat => cat.ParentCategory == null && cat.BookType.Id == bookType.Id)
                        .SingleOrDefault<Category>();

                if (rootCategoryWithBookType != null && rootCategoryWithBookType.Id != rootCategory.Id)
                {
                    throw new BookTypeIsAlreadyAssociatedWithAnotherCategoryException(bookType.Id,
                        rootCategoryWithBookType.Id);
                }

                var categoryToSave = session.Merge(rootCategory);
                categoryToSave.BookType = bookType;
                session.Update(categoryToSave);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<Category> FindCategoriesByBookTypeii(BookTypeEnum type)
        {
            using (var session = GetSession())
            {
                Category categoryAlias = null;
                BookType bookTypeAlias = null;

                var rootCategory = session.QueryOver(() => categoryAlias)
                    .JoinAlias(x => x.BookType, () => bookTypeAlias)
                    .Where(() => bookTypeAlias.Type == type && categoryAlias.ParentCategory == null)
                    .SingleOrDefault<Category>();

                if (rootCategory == null)
                {
                    return new List<Category>();
                }

                var resultCategories = new List<Category>();
                IList<Category> childCategories = new List<Category> {rootCategory};

                while (childCategories != null && childCategories.Count() != 0)
                {
                    resultCategories.AddRange(childCategories);

                    IList<int> parentCategoriesIds = childCategories.Select(childCategory => childCategory.Id).ToList();
                    var ids = parentCategoriesIds;
                    childCategories = session.QueryOver(() => categoryAlias)
                        .Where(() => categoryAlias.ParentCategory.Id.IsIn(ids.ToArray()))
                        .List<Category>();
                }

                return resultCategories;
            }
        }

        public virtual IList<Category> FindCategoriesByBookType(BookTypeEnum type)
        {
            using (var session = GetSession())
            {
                Category categoryAlias = null;
                BookType bookTypeAlias = null;

                var rootCategory = session.QueryOver(() => categoryAlias)
                    .JoinAlias(x => x.BookType, () => bookTypeAlias)
                    .Where(() => bookTypeAlias.Type == type && categoryAlias.ParentCategory == null)
                    .SingleOrDefault<Category>();

                if (rootCategory == null)
                    return new List<Category>();

                return session.QueryOver<Category>()
                    .WhereRestrictionOn(x => x.Path)
                    .IsLike(rootCategory.Path, MatchMode.Start)
                    .List<Category>();
            }
        }


        [Transaction(TransactionMode.Requires)]
        public virtual BookType FindBookTypeByCategory(Category category)
        {
            using (var session = GetSession())
            {
                var resultCategory = session.QueryOver<Category>().Where(cat => cat.Id == category.Id).SingleOrDefault();
                return resultCategory == null ? null : resultCategory.BookType;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual BookType FindBookTypeByType(BookTypeEnum bookTypeEnum)
        {
            using (var session = GetSession())
            {
                return session.QueryOver<BookType>()
                    .Where(x => x.Type == bookTypeEnum)
                    .SingleOrDefault();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<int> GetAllSubcategoryIds(IList<int> categoryIds)
        {
            using (var session = GetSession())
            {
                if (categoryIds == null || categoryIds.Count == 0)
                    return new List<int>();

                return session.GetNamedQuery("GetCategoryHierarchy")
                    .SetParameterList("categoryIds", categoryIds)
                    .List<int>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<long> GetBookIdsFromCategory(IList<int> categoryIds)
        {
            using (var session = GetSession())
            {
                return session.GetNamedQuery("GetBookIdsFromCategoryHierarchy")
                    .SetParameterList("categoryIds", categoryIds)
                    .List<long>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<Category> GetRootCategories()
        {
            using (var session = GetSession())
            {
                var categories = session.QueryOver<Category>()
                    .Where(x => x.ParentCategory == null)
                    .List<Category>();
                return categories;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<Category> FindChildCategoriesInCategory(int categoryId)
        {
            using (var session = GetSession())
            {
                var categories = session.QueryOver<Category>()
                    .Where(x => x.ParentCategory.Id == categoryId)
                    .List<Category>();
                return categories;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<BookVersion> FindChildBookVersionsInCategory(int categoryId)
        {
            Book bookAlias = null;
            BookVersion bookVersionAlias = null;
            BookType bookTypeAlias = null;
            Category categoryAlias = null;

            using (var session = GetSession())
            {
                var bookVersions =
                    session.QueryOver(() => bookVersionAlias)
                        .JoinAlias(() => bookVersionAlias.Categories, () => categoryAlias)
                        .JoinAlias(() => categoryAlias.BookType, () => bookTypeAlias)
                        .JoinAlias(() => bookVersionAlias.Book, () => bookAlias)
                        .Where(() => categoryAlias.Id == categoryId && bookVersionAlias.Id == bookAlias.LastVersion.Id)
                        .OrderBy(() => bookVersionAlias.Title).Asc
                        .TransformUsing(Transformers.DistinctRootEntity)
                        .List<BookVersion>();

                return bookVersions;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<Category> GetAllCategories()
        {
            using (var session = GetSession())
            {
                var categories = session.QueryOver<Category>().List<Category>();
                return categories;
            }
        }


        [Transaction(TransactionMode.Requires)]
        public virtual IList<Category> GetDirectCategoriesByBookVersionId(long bookVersionId)
        {
            BookVersion bookVersionAlias = null;
            Category categoryAlias = null;

            using (var session = GetSession())
            {
                var categories =
                    session.QueryOver(() => categoryAlias)
                        .JoinAlias(() => categoryAlias.BookVersions, () => bookVersionAlias)
                        .Where( () => bookVersionAlias.Id == bookVersionId)
                        .List<Category>();
                return categories;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<long> GetAllBookIdsByBookType(BookTypeEnum currentBookType)
        {
            using (var session = GetSession())
            {

                //var topSelect = session.QueryOver<Category>().Where(c=> c.BookType)
                Category catAlias = null;
                BookVersion bookVersionAlias = null;
                Book bookAlias = null;
                return session.QueryOver<Category>(() => catAlias)
                    .JoinQueryOver(c => c.BookType).Where(bt => bt.Type == currentBookType)
                    .JoinQueryOver(() => catAlias.BookVersions, () => bookVersionAlias)
                    .JoinQueryOver(() => bookVersionAlias.Book, () => bookAlias)
                    .Where(() => bookVersionAlias.Id == bookAlias.LastVersion.Id)                    
                    .Select(Projections.Property(() => bookAlias.Id)).List<long>();
            }
        }
    }
}