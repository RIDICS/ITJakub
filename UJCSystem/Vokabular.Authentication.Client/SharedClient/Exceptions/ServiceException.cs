using System;

namespace Vokabular.Authentication.Client.SharedClient.Exceptions
{
    public class ServiceException : Exception
    {
        public ServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public int StatusCode { get; set; }

        public string Content { get; set; }

        public string ContentType { get; set; }
    }
}
