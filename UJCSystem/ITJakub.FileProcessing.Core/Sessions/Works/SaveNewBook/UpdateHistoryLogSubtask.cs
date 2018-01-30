using System;
using ITJakub.FileProcessing.Core.Data;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;

namespace ITJakub.FileProcessing.Core.Sessions.Works.SaveNewBook
{
    public class UpdateHistoryLogSubtask
    {
        private readonly ProjectRepository m_projectRepository;

        public UpdateHistoryLogSubtask(ProjectRepository projectRepository)
        {
            m_projectRepository = projectRepository;
        }
        
        public void UpdateHistoryLog(long projectId, int userId, string comment, BookData bookData)
        {
            var newLog = new FullProjectImportLog
            {
                Project = m_projectRepository.Load<Project>(projectId),
                AdditionalDescription = comment,
                CreateTime = DateTime.UtcNow,
                ExternalId = bookData.VersionXmlId,
                Text = "New book full import",
                User = m_projectRepository.Load<User>(userId)
            };
            m_projectRepository.Create(newLog);
        }
    }
}