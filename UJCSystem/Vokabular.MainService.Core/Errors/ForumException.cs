using System.Net;
using Vokabular.MainService.DataContracts;

namespace Vokabular.MainService.Core.Errors
{
    public class ForumException : MainServiceException
    {
        public ForumException(string code, string description, HttpStatusCode statusCode = HttpStatusCode.BadRequest,
            object[] descriptionParams = null) : base(code, description, statusCode, descriptionParams)
        {
        }
    }
}