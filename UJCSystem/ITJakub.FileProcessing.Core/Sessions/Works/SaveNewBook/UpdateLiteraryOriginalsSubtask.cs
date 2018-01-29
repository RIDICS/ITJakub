using System.Linq;
using ITJakub.FileProcessing.Core.Data;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;

namespace ITJakub.FileProcessing.Core.Sessions.Works.SaveNewBook
{
    public class UpdateLiteraryOriginalsSubtask
    {
        private readonly CatalogValueRepository m_catalogValueRepository;
        private readonly MetadataRepository m_metadataRepository;

        public UpdateLiteraryOriginalsSubtask(CatalogValueRepository catalogValueRepository, MetadataRepository metadataRepository)
        {
            m_catalogValueRepository = catalogValueRepository;
            m_metadataRepository = metadataRepository;
        }

        public void UpdateLiteraryOriginals(long projectId, BookData bookData)
        {
            if (bookData.LiteraryOriginals == null)
                return;

            var dbOriginalList = m_catalogValueRepository.GetLiteraryOriginalList();
            var project = m_metadataRepository.GetAdditionalProjectMetadata(projectId, false, false, false, false, true, false);

            foreach (var newOriginalName in bookData.LiteraryOriginals)
            {
                var dbOriginal = dbOriginalList.FirstOrDefault(x => x.Name == newOriginalName);

                // Create new Literary Original
                if (dbOriginal == null)
                {
                    dbOriginal = new LiteraryOriginal
                    {
                        Name = newOriginalName
                    };
                    m_catalogValueRepository.Create(dbOriginal);
                    dbOriginalList.Add(dbOriginal);
                }

                // Assign Literary Original to project
                if (project.LiteraryOriginals.All(x => x.Id != dbOriginal.Id))
                {
                    project.LiteraryOriginals.Add(dbOriginal);
                }
            }
            m_catalogValueRepository.Update(project);
        }
    }
}