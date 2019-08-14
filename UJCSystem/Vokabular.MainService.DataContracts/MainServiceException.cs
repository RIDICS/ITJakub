using System.Net;
using System.Net.Http;

namespace Vokabular.MainService.DataContracts
{
    public class MainServiceException : HttpRequestException
    {
        public string Code { get; set; }

        public string Description { get; set; }

        public object[] DescriptionParams { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public MainServiceException(string code, string description, HttpStatusCode statusCode = HttpStatusCode.BadRequest, object[] descriptionParams = null)
        {
            Code = code;
            Description = description;
            StatusCode = statusCode;
            DescriptionParams = descriptionParams;
        }
    }
}