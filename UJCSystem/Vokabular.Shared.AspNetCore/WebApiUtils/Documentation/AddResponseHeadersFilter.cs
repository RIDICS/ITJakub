using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Vokabular.Shared.AspNetCore.WebApiUtils.Documentation
{
    public class AddResponseHeadersFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (!context.ApiDescription.TryGetMethodInfo(out var methodInfo))
            {
                return;
            }

            var responseAttributes = methodInfo.GetCustomAttributes().OfType<ProducesResponseTypeHeader>();

            foreach (var attr in responseAttributes)
            {
                var response = operation.Responses.FirstOrDefault(x => x.Key == (attr.StatusCode).ToString(CultureInfo.InvariantCulture)).Value;

                if (response != null)
                {
                    if (response.Headers == null)
                    {
                        response.Headers = new Dictionary<string, Header>();
                    }

                    response.Headers.Add(attr.Name, new Header { Description = attr.Description, Type = attr.Type });
                }
            }
        }
    }
}