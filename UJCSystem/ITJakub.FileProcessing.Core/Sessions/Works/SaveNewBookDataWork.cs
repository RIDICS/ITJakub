using System;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.Sessions.Works.SaveNewBook;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace ITJakub.FileProcessing.Core.Sessions.Works
{
    public class SaveNewBookDataWork : UnitOfWorkBase
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly MetadataRepository m_metadataRepository;
        private readonly CategoryRepository m_categoryRepository;
        private readonly ResourceRepository m_resourceRepository;
        private readonly ResourceSessionDirector m_resourceDirector;
        private readonly BookData m_bookData;
        private readonly long? m_nullableProjectId;
        private readonly string m_message;
        private readonly int m_userId;
        private long m_projectId;

        public SaveNewBookDataWork(ProjectRepository projectRepository, MetadataRepository metadataRepository, CategoryRepository categoryRepository,
            ResourceRepository resourceRepository, ResourceSessionDirector resourceDirector) : base(projectRepository)
        {
            m_projectRepository = projectRepository;
            m_metadataRepository = metadataRepository;
            m_categoryRepository = categoryRepository;
            m_resourceRepository = resourceRepository;
            m_resourceDirector = resourceDirector;
            m_nullableProjectId = resourceDirector.GetSessionInfoValue<long?>(SessionInfo.ProjectId);
            m_bookData = resourceDirector.GetSessionInfoValue<BookData>(SessionInfo.BookData);
            m_message = resourceDirector.GetSessionInfoValue<string>(SessionInfo.Message);
            m_userId = resourceDirector.GetSessionInfoValue<int>(SessionInfo.UserId);
        }

        protected override void ExecuteWorkImplementation()
        {
            m_projectId = new UpdateProjectSubtask(m_projectRepository).UpdateProject(m_nullableProjectId, m_userId, m_bookData);
            
            //TODO update: 1) metadata, authors, editors, category, kind, genre
            //TODO 2) Page list & chapters 3) Headwords 4) Tracks 5) keywords 6) terms 7) transformations

            new UpdateAuthorsSubtask(m_metadataRepository).UpdateAuthors(m_projectId, m_bookData);
            new UpdateResponsiblePersonSubtask(m_metadataRepository).UpdateResponsiblePersonList(m_projectId, m_bookData);
            new UpdateMetadataSubtask(m_metadataRepository).UpdateMetadata(m_projectId, m_userId, m_message, m_bookData);
            //new UpdateCategoriesSubtask(m_categoryRepository).UpdateCategoryList(m_projectId, m_bookData); TODO update database and mapping
            new UpdateLiteraryKindsSubtask(m_metadataRepository).UpdateLiteraryKinds(m_projectId, m_bookData);
            new UpdateLiteraryGenresSubtask(m_metadataRepository).UpdateLiteraryGenres(m_projectId, m_bookData);
            new UpdateKeywordsSubtask(m_metadataRepository).UpdateKeywords(m_projectId, m_bookData);

            new UpdatePagesSubtask(m_resourceRepository).UpdatePages(m_projectId, m_userId, m_message, m_bookData);

            new UpdateHistoryLogSubtask(m_projectRepository).UpdateHistoryLog(m_projectId, m_userId, m_message, m_bookData);

            throw new NotImplementedException();
        }
    }
}
