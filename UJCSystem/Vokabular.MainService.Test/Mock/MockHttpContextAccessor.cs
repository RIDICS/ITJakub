using Microsoft.AspNetCore.Http;

namespace Vokabular.MainService.Test.Mock
{
    internal class MockHttpContextAccessor : IHttpContextAccessor
    {
        public HttpContext HttpContext { get; set; }
    }
}
