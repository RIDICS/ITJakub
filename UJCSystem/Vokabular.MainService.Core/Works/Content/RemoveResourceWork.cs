using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Content
{
    public class RemoveResourceWork : UnitOfWorkBase
    {
        private readonly ResourceRepository m_resourceRepository;
        private readonly long m_resourceId;

        public RemoveResourceWork(ResourceRepository resourceRepository, long resourceId) : base(resourceRepository)
        {
            m_resourceRepository = resourceRepository;
            m_resourceId = resourceId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var resource = m_resourceRepository.FindById<Resource>(m_resourceId);
            var pageResource = m_resourceRepository.GetLatestResourceVersion<PageResource>(m_resourceId);
            var trackResource = m_resourceRepository.GetLatestResourceVersion<TrackResource>(m_resourceId);

            resource.IsRemoved = true;
            m_resourceRepository.Update(resource);

            if (pageResource != null)
            {
                var textResourceVersion = m_resourceRepository.GetLatestPageText(m_resourceId);
                TryRemoveResource(textResourceVersion);

                var imageResourceVersion = m_resourceRepository.GetLatestPageImage(m_resourceId);
                TryRemoveResource(imageResourceVersion);
            }

            if (trackResource != null)
            {
                var audioResourceVersion = m_resourceRepository.GetAudioRecordingsByTrack(m_resourceId);
                TryRemoveResources(audioResourceVersion);
            }
        }

        private void TryRemoveResource(ResourceVersion resourceVersion)
        {
            if (resourceVersion == null)
            {
                return;
            }

            var resource = resourceVersion.Resource;
            resource.IsRemoved = true;
            m_resourceRepository.Update(resource);
        }

        private void TryRemoveResources(IEnumerable<ResourceVersion> resourceVersions)
        {
            foreach (var resourceVersion in resourceVersions)
            {
                TryRemoveResource(resourceVersion);
            }
        }
    }
}