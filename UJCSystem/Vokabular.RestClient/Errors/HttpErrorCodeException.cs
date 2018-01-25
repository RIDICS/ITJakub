using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Vokabular.RestClient.Contracts;

namespace Vokabular.RestClient.Errors
{
    public class HttpErrorCodeException : HttpRequestException
    {
        public HttpErrorCodeException(string message, HttpStatusCode statusCode) : base(message)
        {
            StatusCode = statusCode;
        }

        public HttpErrorCodeException(string message, HttpStatusCode statusCode, List<ValidationErrorContract> validationErrors) : base(message)
        {
            StatusCode = statusCode;
            ValidationErrors = validationErrors;
        }

        public HttpErrorCodeException(string message, Exception exception, HttpStatusCode statusCode) : base(message, exception)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; }

        public List<ValidationErrorContract> ValidationErrors { get; }
    }
}