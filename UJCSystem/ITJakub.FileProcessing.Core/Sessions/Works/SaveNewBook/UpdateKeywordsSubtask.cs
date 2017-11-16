using System.Linq;
using ITJakub.FileProcessing.Core.Data;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;

namespace ITJakub.FileProcessing.Core.Sessions.Works.SaveNewBook
{
    public class UpdateKeywordsSubtask
    {
        private readonly CatalogValueRepository m_catalogValueRepository;
        private readonly ProjectRepository m_projectRepository;

        public UpdateKeywordsSubtask(ProjectRepository projectRepository, CatalogValueRepository catalogValueRepository)
        {
            m_catalogValueRepository = catalogValueRepository;
            m_projectRepository = projectRepository;
        }

        public void UpdateKeywords(long projectId, BookData bookData)
        {
            if (bookData.Keywords == null)
                return;

            var project = m_projectRepository.GetProjectWithKeywords(projectId);

            foreach (var newKeywordName in bookData.Keywords)
            {
                var dbKeyword = m_catalogValueRepository.GetKeywordByName(newKeywordName);

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
            m_projectRepository.Update(project);
        }
    }
}