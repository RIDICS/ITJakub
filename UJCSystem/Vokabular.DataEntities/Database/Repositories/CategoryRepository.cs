using System.Collections.Generic;
using NHibernate.Criterion;
using Vokabular.DataEntities.Database.Daos;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.DataEntities.Database.Repositories
{
    public class CategoryRepository : NHibernateDao
    {
        public CategoryRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public virtual BookType GetBookTypeByEnum(BookTypeEnum bookType)
        {
            return GetSession().QueryOver<BookType>()
                .Where(x => x.Type == bookType)
                .SingleOrDefault();
        }

        public virtual IList<Category> GetCategoryList()
        {
            return GetSession().QueryOver<Category>()
                .OrderBy(x => x.Description).Asc
                .List();
        }

        public virtual Project GetProjectWithCategories(long projectId)
        {
            return GetSession().QueryOver<Project>()
                .Where(x => x.Id == projectId)
                .Fetch(x => x.Categories).Eager
                .SingleOrDefault();
        }

        public virtual IList<int> GetAllSubcategoryIds(IList<int> selectedCategoryIds)
        {
            return GetSession().GetNamedQuery("GetCategoryHierarchy")
                .SetParameterList("categoryIds", selectedCategoryIds)
                .List<int>();
        }

        public virtual Category GetCategoryWithSubcategories(int categoryId)
        {
            return GetSession().QueryOver<Category>()
                .Where(x => x.Id == categoryId)
                .Fetch(x => x.Categories).Eager
                .SingleOrDefault();
        }

        public virtual IList<Category> GetCategoriesByPath(string categoryPath)
        {
            return GetSession().QueryOver<Category>()
                .WhereRestrictionOn(x => x.Path).IsLike(categoryPath, MatchMode.Start)
                .List();
        }

        public virtual long GetAnyProjectIdByCategory(IEnumerable<int> categoryIds)
        {
            Project projectAlias = null;

            return GetSession().QueryOver<Category>()
                .WhereRestrictionOn(x => x.Id).IsInG(categoryIds)
                .JoinAlias(x => x.Projects, () => projectAlias)
                .Select(x => projectAlias.Id)
                .Take(1)
                .SingleOrDefault<long>();
        }
    }
}