using System.Linq;
using System.Net.Http;

namespace Vokabular.Authentication.Client.Contract
{
    public sealed class FileResourceStreamHydrator<TContract> : StreamHydrator<TContract> where TContract : FileResourceStreamContract
    {
        public override void Hydrate(
            TContract responseObject,
            HttpResponseMessage response
        )
        {
            base.Hydrate(responseObject, response);

            var httpResponseHeaders = response.Headers;
            responseObject.FileName = httpResponseHeaders.GetValues("X-FileName").FirstOrDefault();
            responseObject.Guid = httpResponseHeaders.GetValues("X-Guid").FirstOrDefault();
            responseObject.Type = httpResponseHeaders.GetValues("X-Type").FirstOrDefault();
            responseObject.FileExtension = httpResponseHeaders.GetValues("X-FileExtension").FirstOrDefault();
        }
    }
}
