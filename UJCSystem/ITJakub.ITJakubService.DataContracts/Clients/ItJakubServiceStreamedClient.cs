using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.ServiceModel.Channels;
using ITJakub.ITJakubService.DataContracts.Contracts.AudioBooks;
using log4net;

namespace ITJakub.ITJakubService.DataContracts.Clients
{
    public class ItJakubServiceStreamedClient : ClientBase<IItJakubServiceStreamed>, IItJakubServiceStreamed
    {

        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ItJakubServiceStreamedClient(string endpointConfigurationName) : base(endpointConfigurationName)
        {
        }

        public ItJakubServiceStreamedClient(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
        {
        }

        private string GetCurrentMethod([CallerMemberName] string methodName = null)
        {
            return methodName;
        }

        public void AddResource(UploadResourceContract resourceInfoSkeleton)
        {
            try
            {
                Channel.AddResource(resourceInfoSkeleton);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("AddResource failed with: {0}", ex);
                throw;
            }
            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("AddResource failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("AddResource timeouted with: {0}", ex);
                throw;
            }
        }

        public FileDataContract DownloadWholeAudiobook(DownloadWholeBookContract requestContract)
        {
            try
            {
                return Channel.DownloadWholeAudiobook(requestContract);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
        }

        public AudioTrackContract DownloadAudioBookTrack(DownloadAudioBookTrackContract requestContract)
        {
            try
            {
                return Channel.DownloadAudioBookTrack(requestContract);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
        }
    }
}