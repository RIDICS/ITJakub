using Vokabular.ForumSite.DataEntities.Database.Entities;
using Vokabular.Shared.DataEntities.Daos;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ForumSite.DataEntities.Database.Repositories
{
    public class CategoryRepository : NHibernateDao
    {
        public CategoryRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public virtual Category GetCategoryByName(string name)
        {
            return GetSession().QueryOver<Category>()
                .Where(x => x.Name == name)
                .SingleOrDefault();
        }

        public virtual Category CreateOrGetCategoryByName(string name)
        {
            Category category = GetSession().QueryOver<Category>()
                .Where(x => x.Name == name)
                .SingleOrDefault();

            if (category == null)
            {
                category = new Category();
                category.Board = null; //TODO how to get board
                category.Name = name;
                category = (Category) this.Create(category);
            }

            return category;
        }
    }
}
