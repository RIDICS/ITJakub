﻿using System.Collections.Generic;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using Vokabular.DataEntities.Database.Daos;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
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

        public virtual IList<ResponsibleType> GetResponsibleTypeList()
        {
            return GetSession().QueryOver<ResponsibleType>()
                .OrderBy(x => x.Text).Asc
                .List();
        }

        public virtual IList<TermCategory> GetTermCategoryList()
        {
            return GetSession().QueryOver<TermCategory>()
                .OrderBy(x => x.Name).Asc
                .List();
        }

        public virtual Keyword GetKeywordByName(string name)
        {
            return GetSession().QueryOver<Keyword>()
                .Where(x => x.Text == name)
                .SingleOrDefault();
        }

        public virtual IList<Term> GetTermList(int? categoryId = null)
        {
            var query = GetSession().QueryOver<Term>()
                .OrderBy(x => x.Position).Asc;

            if (categoryId != null)
            {
                query.Where(x => x.TermCategory.Id == categoryId.Value);
            }

            return query.List();
        }

        public virtual IList<TermCategory> GetTermCategoriesWithTerms()
        {
            Term termAlias = null;

            return GetSession().QueryOver<TermCategory>()
                .JoinAlias(x => x.Terms, () => termAlias, JoinType.LeftOuterJoin)
                .Fetch(x => x.Terms).Eager
                .OrderBy(() => termAlias.Position).Asc
                .TransformUsing(Transformers.DistinctRootEntity)
                .List();
        }

        public virtual BookType GetBookType(BookTypeEnum bookTypeEnum)
        {
            return GetSession().QueryOver<BookType>()
                .Where(x => x.Type == bookTypeEnum)
                .SingleOrDefault();
        }
    }
}