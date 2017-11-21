﻿using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Vokabular.MainService.Utils
{
    public class AddResponseHeadersFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var responseAttributes = context.ApiDescription.ActionAttributes().OfType<ProducesResponseTypeHeader>();

            foreach (var attr in responseAttributes)
            {
                var response = operation.Responses.FirstOrDefault(x => x.Key == ((int)attr.StatusCode).ToString(CultureInfo.InvariantCulture)).Value;

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