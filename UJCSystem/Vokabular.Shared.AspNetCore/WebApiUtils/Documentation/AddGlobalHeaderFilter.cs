using System.Collections.Generic;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Vokabular.Shared.AspNetCore.WebApiUtils.Documentation
{
    public class AddGlobalHeaderFilter : IOperationFilter
    {
        private readonly GlobalHeaderValues m_headerValues;

        public AddGlobalHeaderFilter(GlobalHeaderValues headerValues)
        {
            m_headerValues = headerValues;
        }

        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<IParameter>();

            operation.Parameters.Add(new NonBodyParameter
            {
                Name = m_headerValues.Name,
                In = "header",
                Type = "string",
                Enum = m_headerValues.Values,
                Default = m_headerValues.DefaultValue,
                Required = false
            });
        }
    }

    public class GlobalHeaderValues
    {
        public string Name { get; set; }
        public List<object> Values { get; set; }
        public string DefaultValue { get; set; }
    }
}