using System.Collections.Generic;
using Vokabular.DataEntities.Database.Daos;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.DataEntities.Database.Repositories
{
    public class CatalogValueRepository : NHibernateDao
    {
        public CatalogValueRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public virtual IList<LiteraryKind> GetLiteraryKindList()
        {
            return GetSession().QueryOver<LiteraryKind>()
                .OrderBy(x => x.Name).Asc
                .List();
        }

        public virtual IList<LiteraryGenre> GetLiteraryGenreList()
        {
            return GetSession().QueryOver<LiteraryGenre>()
                .OrderBy(x => x.Name).Asc
                .List();
        }

        public virtual IList<LiteraryOriginal> GetLiteraryOriginalList()
        {
            return GetSession().QueryOver<LiteraryOriginal>()
                .OrderBy(x => x.Name).Asc
                .List();
        }

        public virtual IList<Keyword> GetKeywordList()
        {
            return GetSession().QueryOver<Keyword>()
                .OrderBy(x => x.Text).Asc
                .List();
        }
    }
}