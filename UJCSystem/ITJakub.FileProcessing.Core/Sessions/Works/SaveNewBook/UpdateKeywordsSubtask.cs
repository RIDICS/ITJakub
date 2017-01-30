using System.Linq;
using ITJakub.FileProcessing.Core.Data;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;

namespace ITJakub.FileProcessing.Core.Sessions.Works.SaveNewBook
{
    public class UpdateKeywordsSubtask
    {
        private readonly MetadataRepository m_metadataRepository;

        public UpdateKeywordsSubtask(MetadataRepository metadataRepository)
        {
            m_metadataRepository = metadataRepository;
        }

        public void UpdateKeywords(long projectId, BookData bookData)
        {
            var project = m_metadataRepository.GetProjectWithKeywords(projectId);

            foreach (var newKeywordName in bookData.Keywords)
            {
                var dbKeyword = m_metadataRepository.GetKeywordByName(newKeywordName);

                // Create new Keyword
                if (dbKeyword == null)
                {
                    dbKeyword = new Keyword
                    {
                        Text = newKeywordName
                    };
                    project.Keywords.Add(dbKeyword);
                }
                // Assign existing Keyword to project
                else if (project.Keywords.All(x => x.Id != dbKeyword.Id))
                {
                    project.Keywords.Add(dbKeyword);
                }
            }
            m_metadataRepository.Update(project);
        }
    }
}