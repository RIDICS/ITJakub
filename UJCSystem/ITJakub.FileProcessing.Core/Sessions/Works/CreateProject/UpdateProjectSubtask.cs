using System;
using System.Collections.Generic;
using ITJakub.FileProcessing.Core.Data;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;

namespace ITJakub.FileProcessing.Core.Sessions.Works.CreateProject
{
    public class UpdateProjectSubtask
    {
        private readonly ProjectRepository m_projectRepository;

        public UpdateProjectSubtask(ProjectRepository projectRepository)
        {
            m_projectRepository = projectRepository;
        }

        public long UpdateProject(long? projectId, int userId, BookData bookData, ProjectTypeEnum projectType)
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
                        ProjectType = projectType,
                        CreateTime = DateTime.UtcNow,
                        CreatedByUser = m_projectRepository.Load<User>(userId),
                        ExternalId = bookData.BookXmlId,
                        Categories = new List<Category>(),
                        Keywords = new List<Keyword>(),
                        LiteraryGenres = new List<LiteraryGenre>(),
                        LiteraryKinds = new List<LiteraryKind>()
                    };
                    projectIdValue = (long)m_projectRepository.Create(newProject);
                }
            }
            return projectIdValue;
        }
    }
}