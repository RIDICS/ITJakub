using Vokabular.DataEntities.Database.Daos;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.DataEntities.Database.Repositories
{
    public class ImportHistoryRepository : NHibernateDao
    {
        public ImportHistoryRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public ImportHistory GetLatestSuccessfulImportHistory(int externalRepositoryId)
        {
            return GetSession().QueryOver<ImportHistory>()
                .Where(x => x.ExternalRepository.Id == externalRepositoryId)
                .AndRestrictionOn(x => x.Status).IsInG(new []{ImportStatusEnum.Completed, ImportStatusEnum.CompletedWithWarnings})
                .OrderBy(x => x.Date).Desc
                .Take(1)
                .SingleOrDefault();
        }
    }
}