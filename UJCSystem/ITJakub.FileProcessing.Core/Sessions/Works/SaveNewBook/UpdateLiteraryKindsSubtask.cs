using System.Linq;
using ITJakub.FileProcessing.Core.Data;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;

namespace ITJakub.FileProcessing.Core.Sessions.Works.SaveNewBook
{
    public class UpdateLiteraryKindsSubtask
    {
        private readonly MetadataRepository m_metadataRepository;

        public UpdateLiteraryKindsSubtask(MetadataRepository metadataRepository)
        {
            m_metadataRepository = metadataRepository;
        }

        public void UpdateLiteraryKinds(long projectId, BookData bookData)
        {
            var dbKindList = m_metadataRepository.GetLiteraryKindList();
            var project = m_metadataRepository.GetAdditionalProjectMetadata(projectId, false, false, true, false);

            foreach (var newKindName in bookData.LiteraryKinds)
            {
                var dbKind = dbKindList.FirstOrDefault(x => x.Name == newKindName);

                // Create new Literary Kind
                if (dbKind == null)
                {
                    dbKind = new LiteraryKind
                    {
                        Name = newKindName
                    };
                    m_metadataRepository.Create(dbKind);
                    dbKindList.Add(dbKind);
                }

                // Assign Literary Kind to project
                if (project.LiteraryKinds.All(x => x.Id != dbKind.Id))
                {
                    project.LiteraryKinds.Add(dbKind);
                }
            }
            m_metadataRepository.Update(project);
        }
    }
}