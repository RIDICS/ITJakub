using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Criterion.Lambda;
using Vokabular.DataEntities.Database.Daos;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Entities.SelectResults;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.DataEntities.Database.Repositories
{
    public class PortalRepository : NHibernateDao
    {
        public PortalRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public virtual ListWithTotalCountResult<NewsSyndicationItem> GetNewsSyndicationItems(int start, int count, SyndicationItemType? type)
        {
            var query = GetSession().QueryOver<NewsSyndicationItem>()
                .Fetch(x => x.User).Eager
                .OrderBy(x => x.CreateTime).Desc;

            if (type != null)
            {
                query = query.Where(x => x.ItemType == type || x.ItemType == SyndicationItemType.Combined);
            }

            var list = query.Skip(start)
                .Take(count)
                .Future();

            var totalCount = query.ToRowCountQuery()
                .FutureValue<int>();

            return new ListWithTotalCountResult<NewsSyndicationItem>
            {
                List = list.ToList(),
                Count = totalCount.Value,
            };
        }

        public ListWithTotalCountResult<Feedback> GetFeedbackList(int start, int count, FeedbackSortEnum sort, SortDirectionEnumContract sortDirection, IList<FeedbackCategoryEnum> filterCategories)
        {
            var query = GetSession().QueryOver<Feedback>();

            IQueryOverOrderBuilder<Feedback, Feedback> queryOrder;

            switch (sort)
            {
                case FeedbackSortEnum.Date:
                    queryOrder = query.OrderBy(x => x.CreateTime);
                    break;
                case FeedbackSortEnum.Category:
                    queryOrder = query.OrderBy(x => x.FeedbackCategory);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sort), sort, null);
            }

            switch (sortDirection)
            {
                case SortDirectionEnumContract.Asc:
                    query = queryOrder.Asc;
                    break;
                case SortDirectionEnumContract.Desc:
                    query = queryOrder.Desc;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sortDirection), sortDirection, null);
            }

            if (sort == FeedbackSortEnum.Category)
            {
                query = query.ThenBy(x => x.CreateTime).Desc;
            }

            if (filterCategories.Count > 0)
            {
                query = query.WhereRestrictionOn(x => x.FeedbackCategory).IsInG(filterCategories);
            }

            var list = query.Skip(start)
                .Take(count)
                .Future();

            var totalCount = query.ToRowCountQuery()
                .FutureValue<int>();

            return new ListWithTotalCountResult<Feedback>
            {
                List = list.ToList(),
                Count = totalCount.Value,
            };
        }
    }
}
