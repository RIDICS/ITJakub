﻿using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Vokabular.Shared;

namespace ITJakub.FileProcessing.DataContracts
{
    public class FileProcessingServiceClient : ClientBase<IFileProcessingService>, IFileProcessingService
    {
        public FileProcessingServiceClient()
        {
        }

        public FileProcessingServiceClient(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
        {
        }

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

        public ImportResult ProcessSession(string sessionId, long? projectId, int userId, string uploadMessage)
        {
            try
            {
                return Channel.ProcessSession(sessionId, projectId, userId, uploadMessage);
            }
            catch (TimeoutException ex)
            {
                throw new FileProcessingException("Communication with FileProcessing service failed. See inner exception.", ex);
            }
            catch (CommunicationException ex)
            {
                throw new FileProcessingException(String.Format("Communication with FileProcessing service failed. See inner exception: {0}", ex.Message), ex);
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