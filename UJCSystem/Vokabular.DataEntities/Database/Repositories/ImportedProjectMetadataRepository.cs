using NHibernate;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.DataEntities.Database.Repositories
{
    public class ImportedProjectMetadataRepository : MainDbRepositoryBase
    {
        public ImportedProjectMetadataRepository(UnitOfWorkProvider unitOfWorkProvider) : base(unitOfWorkProvider)
        {
        }

        public virtual ImportedProjectMetadata GetImportedProjectMetadata(string externalId)
        {
            return GetSession().QueryOver<ImportedProjectMetadata>()
                .Where(x => x.ExternalId == externalId)
                .Fetch(SelectMode.Fetch, x => x.Project)
                .SingleOrDefault();
        }
    }
}