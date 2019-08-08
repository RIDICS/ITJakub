using System;
using System.Net;

namespace Vokabular.MainService.DataContracts
{
    public class MainServiceException : Exception
    {
        public string Code { get; set; }

        public string Description { get; set; }

        public HttpStatusCode StatusCode { get; set; }
    }
}