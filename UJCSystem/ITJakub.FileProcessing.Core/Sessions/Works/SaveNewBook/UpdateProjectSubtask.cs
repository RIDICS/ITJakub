using System;
using ITJakub.FileProcessing.Core.Data;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;

namespace ITJakub.FileProcessing.Core.Sessions.Works.SaveNewBook
{
    public class UpdateProjectSubtask
    {
        private readonly ProjectRepository m_projectRepository;

        public UpdateProjectSubtask(ProjectRepository projectRepository)
        {
            m_projectRepository = projectRepository;
        }

        public long UpdateProject(long? projectId, int userId, BookData bookData)
        {
            long projectIdValue;

            if (projectId != null)
            {
                projectIdValue = projectId.Value;

                var project = m_projectRepository.FindById<Project>(projectIdValue);
                if (project.ExternalId != bookData.BookXmlId)
                {
                    project.ExternalId = bookData.BookXmlId;
                    m_projectRepository.Update(project);
                }
            }
            else
            {
                var dbProject = m_projectRepository.GetProjectByExternalId(bookData.BookXmlId);
                if (dbProject != null)
                {
                    projectIdValue = dbProject.Id;
                }
                else
                {
                    var newProject = new Project
                    {
                        Name = bookData.Title,
                        CreateTime = DateTime.UtcNow,
                        CreatedByUser = m_projectRepository.Load<User>(userId),
                        ExternalId = bookData.BookXmlId,
                    };
                    projectIdValue = (int)m_projectRepository.Create(newProject);
                }
            }
            return projectIdValue;
        }
    }
}