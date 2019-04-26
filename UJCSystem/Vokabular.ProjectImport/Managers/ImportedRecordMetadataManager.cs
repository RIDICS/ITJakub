using Vokabular.DataEntities.Database.Repositories;
using Vokabular.ProjectImport.Works;
using Vokabular.ProjectParsing.Model.Entities;

namespace Vokabular.ProjectImport.Managers
{
    public class ImportedRecordMetadataManager
    {
        private readonly ImportedRecordMetadataRepository m_importedRecordMetadataRepository;

        public ImportedRecordMetadataManager(ImportedRecordMetadataRepository importedRecordMetadataRepository)
        {
            m_importedRecordMetadataRepository = importedRecordMetadataRepository;
        }

        public int CreateImportedRecordMetadata(ImportedRecord importedRecord, int importHistoryId)
        {
            var result = new CreateImportedRecordMetadataWork(m_importedRecordMetadataRepository, importedRecord, importHistoryId).Execute();
            return result;
        }
    }
}