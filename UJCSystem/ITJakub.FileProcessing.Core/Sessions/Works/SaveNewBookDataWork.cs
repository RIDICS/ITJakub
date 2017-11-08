using System;
using System.Collections.Generic;
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
        private readonly ResourceRepository m_resourceRepository;
        private readonly BookData m_bookData;
        private readonly long? m_nullableProjectId;
        private readonly string m_message;
        private readonly int m_userId;
        private long m_projectId;
        private long m_bookVersionId;
        private List<long> m_importedResourceVersionIds;

        public SaveNewBookDataWork(ProjectRepository projectRepository, MetadataRepository metadataRepository,
            ResourceRepository resourceRepository, ResourceSessionDirector resourceDirector) : base(projectRepository)
        {
            m_projectRepository = projectRepository;
            m_metadataRepository = metadataRepository;
            m_resourceRepository = resourceRepository;
            m_nullableProjectId = resourceDirector.GetSessionInfoValue<long?>(SessionInfo.ProjectId);
            m_bookData = resourceDirector.GetSessionInfoValue<BookData>(SessionInfo.BookData);
            m_message = resourceDirector.GetSessionInfoValue<string>(SessionInfo.Message);
            m_userId = resourceDirector.GetSessionInfoValue<int>(SessionInfo.UserId);
        }

        protected override void ExecuteWorkImplementation()
        {
            m_projectId = m_nullableProjectId ?? throw new InvalidOperationException("Required ProjectId wasn't set in ResourceSessionDirector");
            // Updating Project data is not required, because data has been already updated

            m_importedResourceVersionIds = new List<long>();

            m_bookVersionId = new UpdateBookVersionSubtask(m_resourceRepository).UpdateBookVersion(m_projectId, m_userId, m_message, m_bookData);
            
            //TODO update: transformations

            new UpdateAuthorsSubtask(m_metadataRepository).UpdateAuthors(m_projectId, m_bookData);
            new UpdateResponsiblePersonSubtask(m_metadataRepository).UpdateResponsiblePersonList(m_projectId, m_bookData);

            var updateMetadataSubtask = new UpdateMetadataSubtask(m_metadataRepository);
            updateMetadataSubtask.UpdateMetadata(m_projectId, m_userId, m_message, m_bookData);

            // Categories are not updated from import (XMD doesn't contain detailed categorization)
            //new UpdateCategoriesSubtask(m_categoryRepository).UpdateCategoryList(m_projectId, m_bookData);

            new UpdateLiteraryKindsSubtask(m_metadataRepository).UpdateLiteraryKinds(m_projectId, m_bookData);
            new UpdateLiteraryGenresSubtask(m_metadataRepository).UpdateLiteraryGenres(m_projectId, m_bookData);
            new UpdateLiteraryOriginalsSubtask(m_metadataRepository).UpdateLiteraryOriginals(m_projectId, m_bookData);
            new UpdateKeywordsSubtask(m_metadataRepository).UpdateKeywords(m_projectId, m_bookData);

            var updateTermsSubtask = new UpdateTermsSubtask(m_resourceRepository);
            updateTermsSubtask.UpdateTerms(m_projectId, m_bookData);

            var updatePagesSubtask = new UpdatePagesSubtask(m_resourceRepository);
            updatePagesSubtask.UpdatePages(m_projectId, m_bookVersionId, m_userId, m_message, m_bookData, updateTermsSubtask.ResultTermCache);
            m_importedResourceVersionIds.AddRange(updatePagesSubtask.ImportedResourceVersionIds);

            var updateChaptersSubtask = new UpdateChaptersSubtask(m_resourceRepository);
            updateChaptersSubtask.UpdateChapters(m_projectId, m_userId, m_message, m_bookData, updatePagesSubtask.ResultPageResourceList);
            m_importedResourceVersionIds.AddRange(updateChaptersSubtask.ImportedResourceVersionIds);

            var updateHeadwordsSubtask = new UpdateHeadwordsSubtask(m_resourceRepository);
            updateHeadwordsSubtask.UpdateHeadwords(m_projectId, m_bookVersionId, m_userId, m_message, m_bookData, updatePagesSubtask.ResultPageResourceList);
            m_importedResourceVersionIds.AddRange(updateHeadwordsSubtask.ImportedResourceVersionIds);

            var updateTracksSubtask = new UpdateTracksSubtask(m_resourceRepository);
            updateTracksSubtask.UpdateTracks(m_projectId, m_userId, m_message, m_bookData);
            updateTracksSubtask.UpdateFullBookTracks(m_projectId, m_userId, m_message, m_bookData);
            m_importedResourceVersionIds.AddRange(updateTracksSubtask.ImportedResourceVersionIds);
            
            new UpdateHistoryLogSubtask(m_projectRepository).UpdateHistoryLog(m_projectId, m_userId, m_message, m_bookData);
        }

        public string Message => m_message;

        public int UserId => m_userId;

        public long ProjectId => m_projectId;

        public List<long> ImportedResourceVersionIds => m_importedResourceVersionIds;

        public BookData BookData => m_bookData;

        public long BookVersionId => m_bookVersionId;
    }
}
