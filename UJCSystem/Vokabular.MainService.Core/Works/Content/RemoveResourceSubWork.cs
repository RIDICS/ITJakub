using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;

namespace Vokabular.MainService.Core.Works.Content
{
    public class RemoveResourceSubwork
    {
        private readonly ResourceRepository m_resourceRepository;

        public RemoveResourceSubwork(ResourceRepository resourceRepository)
        {
            m_resourceRepository = resourceRepository;
        }

        public void RemoveResource(long resourceId)
        {
            var resource = m_resourceRepository.FindById<Resource>(resourceId);
            var pageResource = m_resourceRepository.GetLatestResourceVersion<PageResource>(resourceId);
            var trackResource = m_resourceRepository.GetLatestResourceVersion<TrackResource>(resourceId);

            resource.IsRemoved = true;
            m_resourceRepository.Update(resource);

            if (pageResource != null)
            {
                var textResourceVersion = m_resourceRepository.GetLatestPageText(resourceId);
                TryRemoveResource(textResourceVersion);

                var imageResourceVersion = m_resourceRepository.GetLatestPageImage(resourceId);
                TryRemoveResource(imageResourceVersion);
            }

            if (trackResource != null)
            {
                var audioResourceVersion = m_resourceRepository.GetAudioRecordingsByTrack(resourceId);
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