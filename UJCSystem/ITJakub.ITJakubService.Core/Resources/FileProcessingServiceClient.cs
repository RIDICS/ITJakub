using System;
using System.ServiceModel;
using ITJakub.FileProcessing.DataContracts;
using ITJakub.Shared.Contracts.Resources;

namespace ITJakub.ITJakubService.Core.Resources
{
    public class FileProcessingServiceClient : ClientBase<IFileProcessingService>, IFileProcessingService
    {
        public void AddResource(UploadResourceContract resourceInfoSkeleton)
        {
            try
            {
                Channel.AddResource(resourceInfoSkeleton);
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

        public bool ProcessSession(string sessionId, string uploadMessage)
        {
            try
            {
                return Channel.ProcessSession(sessionId, uploadMessage);
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