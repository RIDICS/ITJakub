using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using System.ServiceModel;
using ITJakub.CardFile.Core.DataContractEntities;
using log4net;

namespace ITJakub.CardFile.Core
{
    public class CardFilesCommunicationManager:IDisposable
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        //TODO move to conf
        private const string UserName = "api";
        private const string UserPassword = "***REMOVED***";

        private CardFilesServiceClient m_serviceClient;

        public CardFilesCommunicationManager()
        {
            if (m_log.IsDebugEnabled)
                m_log.DebugFormat("Creating CardFiles client");

            m_serviceClient = new CardFilesServiceClient();

            if (m_serviceClient.ClientCredentials != null)
            {
                m_serviceClient.ClientCredentials.HttpDigest.ClientCredential = new NetworkCredential(UserName, UserPassword);

                m_serviceClient.ClientCredentials.HttpDigest.AllowedImpersonationLevel = TokenImpersonationLevel.Impersonation;
            }else {
                const string message = "Service client credentials not initialized";

                if (m_log.IsFatalEnabled)
                    m_log.FatalFormat(message);

                throw new InvalidDataException(message);
            }
        }


        public files GetFiles()
        {
            return m_serviceClient.GetFiles();
        }

        public buckets GetBuckets(string fileId, string keyword)
        {
            return m_serviceClient.GetBuckets(fileId, keyword);
        }

        public buckets GetCardsFromBucket(string fileId, string bucketId)
        {
            return m_serviceClient.GetCardsFromBucket(fileId, bucketId);
        }

        public card GetCardFromBucket(string fileId, string bucketId, string cardId)
        {
            return m_serviceClient.GetCardFromBucket(fileId, bucketId, cardId);
        }

        public Image GetImageForCard(string fileId, string bucketId, string cardId, string imageId, string imageSize)
        {
            Stream imageStream = m_serviceClient.GetImageForCard(fileId, bucketId, cardId, imageId, imageSize);
            MemoryStream memImageStream = new MemoryStream();
            imageStream.CopyTo(memImageStream);

            return Image.FromStream(memImageStream);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~CardFilesCommunicationManager() 
    {
        // Finalizer calls Dispose(false)
        Dispose(false);
    }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (m_serviceClient != null)
                {
                    if (m_serviceClient.State == CommunicationState.Faulted)
                    {
                        m_serviceClient.Abort();
                    }
                    else
                    {
                        m_serviceClient.Close();
                    }
                    m_serviceClient = null;
                }
            }
        }
    }
}