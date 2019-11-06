using System;
using System.Collections.Generic;
using System.Linq;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Managers.Fulltext;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Snapshot
{
    public class CreateSnapshotWork : UnitOfWorkBase<long>
    {
        private const int BatchSize = 100;
        private readonly ProjectRepository m_projectRepository;
        private readonly ResourceRepository m_resourceRepository;
        private readonly long m_projectId;
        private readonly int m_userId;
        private readonly IList<long> m_resourceVersionIds;
        private readonly string m_comment;
        private readonly IList<BookTypeEnum> m_bookTypes;
        private readonly BookTypeEnum m_defaultBookType;
        private readonly IFulltextStorage m_fulltextStorage;

        public CreateSnapshotWork(ProjectRepository projectRepository, ResourceRepository resourceRepository, long projectId, int userId, IList<long> resourceVersionIds,
            string comment, IList<BookTypeEnum> bookTypes, BookTypeEnum defaultBookType, IFulltextStorage fulltextStorage) : base(projectRepository)
        {
            m_projectRepository = projectRepository;
            m_resourceRepository = resourceRepository;
            m_projectId = projectId;
            m_userId = userId;
            m_resourceVersionIds = resourceVersionIds;
            m_comment = comment;
            m_bookTypes = bookTypes;
            m_defaultBookType = defaultBookType;
            m_fulltextStorage = fulltextStorage;
        }

        protected override long ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;
            var user = m_projectRepository.Load<User>(m_userId);
            var project = m_projectRepository.Load<Project>(m_projectId);

            var latestSnapshot = m_projectRepository.GetLatestSnapshot(m_projectId);
            var bookTypes = m_bookTypes.Select(bookTypeEnum => m_projectRepository.GetBookTypeByEnum(bookTypeEnum)).ToList();
            var defaultBookType = m_projectRepository.GetBookTypeByEnum(m_defaultBookType);
            var versionNumber = latestSnapshot?.VersionNumber ?? 0;

            // Get all resources (specified in request)

            var resourceVersions = new List<ResourceVersion>();

            var textResourceVersions = BatchGetResourceVersions<TextResource>(m_resourceVersionIds);
            resourceVersions.AddRange(textResourceVersions);

            var imageResourceVersions = BatchGetResourceVersions<ImageResource>(m_resourceVersionIds);
            resourceVersions.AddRange(imageResourceVersions);

            var audioResourceVersions = BatchGetResourceVersions<AudioResource>(m_resourceVersionIds);
            resourceVersions.AddRange(audioResourceVersions);

            // Get required related resources

            var trackResourceVersions = GetTrackResourceVersions(audioResourceVersions);
            resourceVersions.AddRange(trackResourceVersions);

            var pageResourceVersions = GetPageResourceVersions(textResourceVersions, imageResourceVersions, trackResourceVersions);
            resourceVersions.AddRange(pageResourceVersions);

            var chapterResourceVersions = GetChapterResourceVersions(pageResourceVersions);
            resourceVersions.AddRange(chapterResourceVersions);

            // Get all required resources

            var metadataResourceVersion = m_resourceRepository.GetLatestMetadata(m_projectId);
            resourceVersions.Add(metadataResourceVersion);

            var editionNote = m_resourceRepository.GetLatestEditionNote(m_projectId);
            if (editionNote != null)
            {
                resourceVersions.Add(editionNote);
            }

            // Headwords are not loaded because they are not supported in project editor
            // BinaryResource is not used at all now
            // BookVersionResource is not used in publish process (it is used only for eXist-db)

            // Create snapshot

            var newDbSnapshot = new DataEntities.Database.Entities.Snapshot
            {
                Project = project,
                BookTypes = bookTypes,
                DefaultBookType = defaultBookType,
                Comment = m_comment,
                CreateTime = now,
                PublishTime = now,
                CreatedByUser = user,
                VersionNumber = versionNumber + 1,
                ResourceVersions = resourceVersions
            };

            project.LatestPublishedSnapshot = newDbSnapshot;
            m_projectRepository.Update(project);

            var snapshotId = (long)m_projectRepository.Create(newDbSnapshot);

            // Publish snapshot to fulltext database
            var orderedTextResourceVersions =
                textResourceVersions.OrderBy(x => ((PageResource) x.ResourcePage.LatestVersion).Position).ToList();

            m_fulltextStorage.CreateSnapshot(newDbSnapshot, orderedTextResourceVersions, metadataResourceVersion);

            return snapshotId;
        }

        private List<T> BatchGetResourceVersions<T>(IList<long> resourceVersionIds) where T : ResourceVersion
        {
            var resultList = new List<T>();
            for (int i = 0; i < resourceVersionIds.Count; i += BatchSize)
            {
                var ids = resourceVersionIds.Skip(i).Take(BatchSize);
                var dbResultPart = m_resourceRepository.GetResourceVersions<T>(ids);

                resultList.AddRange(dbResultPart);
            }

            return resultList;
        }

        private List<T> BatchGetLatestResourceVersions<T>(IList<long> resourceIds) where T : ResourceVersion
        {
            var resultList = new List<T>();
            for (int i = 0; i < resourceIds.Count; i += BatchSize)
            {
                var ids = resourceIds.Skip(i).Take(BatchSize);
                var dbResult = m_resourceRepository.GetLatestResourceVersions<T>(ids);

                resultList.AddRange(dbResult);
            }

            return resultList;
        }

        private IList<PageResource> GetPageResourceVersions(IList<TextResource> textResources, IList<ImageResource> imageResources, IList<TrackResource> trackResources)
        {
            var allResourceIds = new List<long>();
            allResourceIds.AddRange(textResources.Select(x => x.ResourcePage.Id));
            allResourceIds.AddRange(imageResources.Select(x => x.ResourcePage.Id));
            allResourceIds.AddRange(trackResources.Where(x => x.ResourceBeginningPage != null).Select(x => x.ResourceBeginningPage.Id));
            var resourceIds = allResourceIds.Distinct().ToList();

            var resultList = BatchGetLatestResourceVersions<PageResource>(resourceIds);

            return resultList;
        }

        private IList<ChapterResource> GetChapterResourceVersions(IList<PageResource> pageResources)
        {
            var resourcePageIds = pageResources.Select(x => x.Resource.Id);
            var chapterResourceVersions = m_resourceRepository.GetLatestChaptersByPages(resourcePageIds);

            return chapterResourceVersions;
        }

        private IList<TrackResource> GetTrackResourceVersions(IList<AudioResource> audioResources)
        {
            var trackResourceVersions =
                m_resourceRepository.GetLatestResourceVersions<TrackResource>(audioResources.Select(x => x.ResourceTrack.Id));
            return trackResourceVersions;
        }
    }
}