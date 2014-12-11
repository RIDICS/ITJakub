using System;
using System.IO;
using System.ServiceModel;
using ITJakub.FileProcessing.DataContracts;

namespace ITJakub.ITJakubService.Core.Resources
{
    public class FileProcessingServiceClient : ClientBase<IFileProcessingService>, IFileProcessingService
    {
        public void AddResource(string sessionId, string fileName, Stream dataStream)
        {
            try
            {
                Channel.AddResource(sessionId, fileName, dataStream);
            }
            catch (TimeoutException ex)
            {
                throw new FileProcessingException("Communication with FileProcessing service failed. See inner exception.",ex);
            }                
            catch (CommunicationException ex)
            {
                throw new FileProcessingException("Communication with FileProcessing service failed. See inner exception.", ex);
            }
            catch (ObjectDisposedException ex)
            {
                throw new FileProcessingException("Communication with FileProcessing service failed. See inner exception.", ex);
            }
        }

        public bool ProcessSession(string sessionId)
        {
            try
            {
                return Channel.ProcessSession(sessionId);
            }
            catch (TimeoutException ex)
            {
                throw new FileProcessingException("Communication with FileProcessing service failed. See inner exception.", ex);
            }
            catch (CommunicationException ex)
            {
                throw new FileProcessingException("Communication with FileProcessing service failed. See inner exception.", ex);
            }
            catch (ObjectDisposedException ex)
            {
                throw new FileProcessingException("Communication with FileProcessing service failed. See inner exception.", ex);
            }
        }
    }

    public class FileProcessingException : Exception
    {
        public FileProcessingException(string message,Exception innerException):base(message, innerException)
        {            
        }
    }
}