using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.Repositories.BibliographyImport;
using Vokabular.ProjectImport.Works;
using Vokabular.ProjectParsing.Model.Entities;

namespace Vokabular.ProjectImport.Managers
{
    public class ImportedRecordMetadataManager
    {
        private readonly ImportedRecordMetadataRepository m_importedRecordMetadataRepository;
        private readonly ProjectRepository m_projectRepository;

        public ImportedRecordMetadataManager(ImportedRecordMetadataRepository importedRecordMetadataRepository, ProjectRepository projectRepository)
        {
            m_importedRecordMetadataRepository = importedRecordMetadataRepository;
            m_projectRepository = projectRepository;
        }

        public int CreateImportedRecordMetadata(ImportedRecord importedRecord, int importHistoryId)
        {
            var result = new CreateImportedRecordMetadataWork(m_importedRecordMetadataRepository, m_projectRepository, importedRecord, importHistoryId).Execute();
            return result;
        }
    }
}