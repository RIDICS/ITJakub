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
    public class BookServiceClient : ClientBase<IMobileAppsService>, IBookServiceClient
    {
        //private const string EndpointAddress = "http://localhost/ITJakub.ITJakubService/MobileApps.svc";
        private const string EndpointAddress = "http://censeo2.felk.cvut.cz/ITJakub.ITJakubService/MobileApps.svc";

        public BookServiceClient() : base(GetDefaultBinding(), GetDefaultEndpointAddress())
        {
            
        }
        
        public void UpdateEndpointAddress(string newEndpointAddress)
        {
            Endpoint.Address = new EndpointAddress(newEndpointAddress ?? EndpointAddress);
        }

        public Task<IList<BookContract>> GetBookListAsync(BookTypeContract category)
        {
            return Task.Run(() =>
            {
                try
                {
                    return Channel.GetBookList(category);
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
            });
        }

        public Task<IList<BookContract>> SearchForBookAsync(BookTypeContract category, SearchDestinationContract searchBy, string query)
        {
            return Task.Run(() =>
            {
                try
                {
                    return Channel.SearchForBook(category, searchBy, query);
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
            });
        }

        public Task<IList<PageContract>> GetPageListAsync(string bookGuid)
        {
            return Task.Run(() =>
            {
                try
                {
                    return Channel.GetPageList(bookGuid);
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
            });
        }

        public Task<string> GetPageAsRtfAsync(string bookGuid, string pageId)
        {
            return Task.Run(() =>
            {
                try
                {
                    return Channel.GetPageAsRtf(bookGuid, pageId);
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
            });
        }

        public Task<Stream> GetPagePhotoAsync(string bookGuid, string pageId)
        {
            return Task.Run(() =>
            {
                try
                {
                    return Channel.GetPagePhoto(bookGuid, pageId);
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
            });
        }

        public Task<BookContract> GetBookInfo(string bookGuid)
        {
            return Task.Run(() =>
            {
                try
                {
                    return Channel.GetBookInfo(bookGuid);
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
            });
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
