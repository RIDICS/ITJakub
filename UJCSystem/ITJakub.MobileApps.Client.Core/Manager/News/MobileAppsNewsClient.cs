using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;
using ITJakub.MobileApps.MobileContracts;
using ITJakub.MobileApps.MobileContracts.News;

namespace ITJakub.MobileApps.Client.Core.Manager.News
{
    public class MobileAppsNewsClient: ClientBase<INewsService> , INewsService
    {
        //private const string EndpointAddress = "http://localhost/ITJakub.ITJakubService/MobileApps.svc";
        //private const string EndpointAddress = "http://147.32.81.136/ITJakub.ITJakubService/MobileApps.svc";
        private const string EndpointAddress = "http://censeo2.felk.cvut.cz/ITJakub.ITJakubService/MobileApps.svc";

        public MobileAppsNewsClient() : base(GetDefaultBinding(), GetDefaultEndpointAddress())
        {
        }

        public IList<NewsSyndicationItemContract> GetNewsForMobileApps(int start, int count)
        {         
            try
            {
                return Channel.GetNewsForMobileApps(start, count);
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