using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using System.Xml;
using ITJakub.MobileApps.MobileContracts;

namespace ITJakub.MobileApps.Client.Books.Service.Client
{
    public class ServiceClient : ClientBase<IMobileAppsService>, IMobileAppsService
    {
        private const string EndpointAddress = "http://localhost:11186/MobileApps.svc";
        //private const string EndpointAddress = "http://147.32.81.136/ITJakub.ITJakubService/MobileApps.svc";

        public ServiceClient() : base(GetDefaultBinding(), GetDefaultEndpointAddress())
        {
            
        }

        public async Task<IList<BookContract>> GetBookListAsync(BookTypeContract category)
        {
            try
            {
                return await Channel.GetBookListAsync(category);
            }
            catch (FaultException ex)
            {
                throw new MobileCommunicationException(ex);
            }
            catch (CommunicationException ex)
            {
                throw new MobileCommunicationException(ex);
            }
            catch (TimeoutException ex)
            {
                throw new MobileCommunicationException(ex);
            }
            catch (ObjectDisposedException ex)
            {
                throw new MobileCommunicationException(ex);
            }
        }

        public async Task<IList<BookContract>> SearchForBookAsync(BookTypeContract category, SearchDestinationContract searchBy, string query)
        {
            try
            {
                return await Channel.SearchForBookAsync(category, searchBy, query);
            }
            catch (FaultException ex)
            {
                throw new MobileCommunicationException(ex);
            }
            catch (CommunicationException ex)
            {
                throw new MobileCommunicationException(ex);
            }
            catch (TimeoutException ex)
            {
                throw new MobileCommunicationException(ex);
            }
            catch (ObjectDisposedException ex)
            {
                throw new MobileCommunicationException(ex);
            }
        }

        public async Task<IList<PageContract>> GetPageListAsync(string bookGuid)
        {
            try
            {
                return await Channel.GetPageListAsync(bookGuid);
            }
            catch (FaultException ex)
            {
                throw new NotFoundException(ex);
            }
            catch (CommunicationException ex)
            {
                throw new MobileCommunicationException(ex);
            }
            catch (TimeoutException ex)
            {
                throw new MobileCommunicationException(ex);
            }
            catch (ObjectDisposedException ex)
            {
                throw new MobileCommunicationException(ex);
            }
        }

        public async Task<string> GetPageAsRtfAsync(string bookGuid, string pageId)
        {
            try
            {
                return await Channel.GetPageAsRtfAsync(bookGuid, pageId);
            }
            catch (FaultException ex)
            {
                throw new NotFoundException(ex);
            }
            catch (CommunicationException ex)
            {
                throw new MobileCommunicationException(ex);
            }
            catch (TimeoutException ex)
            {
                throw new MobileCommunicationException(ex);
            }
            catch (ObjectDisposedException ex)
            {
                throw new MobileCommunicationException(ex);
            }
        }

        public async Task<Stream> GetPagePhotoAsync(string bookGuid, string pageId)
        {
            try
            {
                return await Channel.GetPagePhotoAsync(bookGuid, pageId);
            }
            catch (FaultException ex)
            {
                throw new NotFoundException(ex);
            }
            catch (CommunicationException ex)
            {
                throw new MobileCommunicationException(ex);
            }
            catch (TimeoutException ex)
            {
                throw new MobileCommunicationException(ex);
            }
            catch (ObjectDisposedException ex)
            {
                throw new MobileCommunicationException(ex);
            }
        }

        #region enpoint settings
        private static Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.BasicHttpBindingIMobileAppsService))
            {
                var result = new BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                return result;
            }
            throw new InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.",
                endpointConfiguration));
        }

        private static EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.BasicHttpBindingIMobileAppsService))
            {
                return new EndpointAddress(EndpointAddress);
            }
            throw new InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.",
                endpointConfiguration));
        }

        private static Binding GetDefaultBinding()
        {
            return
                GetBindingForEndpoint(EndpointConfiguration.BasicHttpBindingIMobileAppsService);
        }

        private static EndpointAddress GetDefaultEndpointAddress()
        {
            return GetEndpointAddress(EndpointConfiguration.BasicHttpBindingIMobileAppsService);
        }
        #endregion

    }
}
