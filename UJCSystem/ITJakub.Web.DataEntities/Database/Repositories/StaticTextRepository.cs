using ITJakub.Web.DataEntities.Database.Daos;
using ITJakub.Web.DataEntities.Database.Entities;
using ITJakub.Web.DataEntities.Database.UnitOfWork;

namespace ITJakub.Web.DataEntities.Database.Repositories
{
    public class StaticTextRepository : NHibernateDao
    {
        public StaticTextRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        
        public virtual StaticText GetStaticText(string name)
        {
            return GetSession().QueryOver<StaticText>()
                .Where(x => x.Name == name)
                .SingleOrDefault();
        }
    }
}
