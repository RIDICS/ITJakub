using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.DataEntities.Database.Repositories
{
    public class CategoryRepository : MainDbRepositoryBase
    {
        public CategoryRepository(UnitOfWorkProvider unitOfWorkProvider) : base(unitOfWorkProvider)
        {
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
                .Fetch(SelectMode.Fetch, x => x.Categories)
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
                .Fetch(SelectMode.Fetch, x => x.Categories)
                .SingleOrDefault();
        }

        public virtual IList<Category> GetCategoriesWithSubcategories()
        {
            Category subcategoryAlias = null;

            return GetSession().QueryOver<Category>()
                .JoinAlias(x => x.Categories, () => subcategoryAlias, JoinType.LeftOuterJoin)
                .OrderBy(x => x.Description).Asc
                .OrderBy(() => subcategoryAlias.Description).Asc
                .Fetch(SelectMode.Fetch, x => x.Categories)
                .TransformUsing(Transformers.DistinctRootEntity)
                .List()
                .Where(x => x.ParentCategory == null)
                .ToList();
        }

        public virtual IList<Category> GetCategoriesByPath(string categoryPath)
        {
            return GetSession().QueryOver<Category>()
                .WhereRestrictionOn(x => x.Path).IsLike(categoryPath, MatchMode.Start)
                .List();
        }
    }
}