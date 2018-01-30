using Castle.Windsor;
using ITJakub.ITJakubService.Core;
using ITJakub.ITJakubService.Core.Resources;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.ITJakubService.DataContracts.Contracts.AudioBooks;

namespace ITJakub.ITJakubService.Services
{
    public class IItJakubServiceStreamedManager : IItJakubServiceStreamed {

        private readonly WindsorContainer m_container = Container.Current;

        private readonly AudioBookManager m_audioBookManager;
        private readonly ResourceManager m_resourceManager;

        public IItJakubServiceStreamedManager()
        {
            m_audioBookManager = m_container.Resolve<AudioBookManager>();
            m_resourceManager = m_container.Resolve<ResourceManager>();
        }

        public void AddResource(UploadResourceContract resourceInfoSkeleton)
        {
            m_resourceManager.AddResource(resourceInfoSkeleton);
        }

        public FileDataContract DownloadWholeAudiobook(DownloadWholeBookContract requestContract)
        {
            return m_audioBookManager.DownloadWholeAudioBook(requestContract);
        }

        public AudioTrackContract DownloadAudioBookTrack(DownloadAudioBookTrackContract requestContract)
        {
            return m_audioBookManager.DownloadAudioBookTrack(requestContract);
        }
    }
}