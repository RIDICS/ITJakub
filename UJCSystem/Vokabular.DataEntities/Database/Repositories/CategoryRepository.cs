﻿using System.Collections.Generic;
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
                .Fetch(x => x.BookType).Eager
                .List();
        }

        public virtual Project GetProjectWithCategories(long projectId)
        {
            return GetSession().QueryOver<Project>()
                .Where(x => x.Id == projectId)
                .Fetch(x => x.Categories).Eager
                .SingleOrDefault();
        }
    }
}