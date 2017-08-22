using System;
using System.Net;
using System.Net.Http;

namespace Vokabular.MainService.DataContracts.Clients.Errors
{
    public class HttpErrorCodeException : HttpRequestException
    {
        public HttpErrorCodeException(string message, HttpStatusCode statusCode) : base(message)
        {
            StatusCode = statusCode;
        }

        public HttpErrorCodeException(string message, Exception exception, HttpStatusCode statusCode) : base(message, exception)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; }
    }
}