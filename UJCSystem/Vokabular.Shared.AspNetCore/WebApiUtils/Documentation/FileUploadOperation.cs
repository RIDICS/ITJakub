using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Vokabular.Shared.AspNetCore.WebApiUtils.Documentation
{
    // Source: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/193
    // WORKAROUND Swashbuckle should support IFormFile natively but it doesn't work in ASP.NET Core 2.2
    public class FileOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            for (var i = 0; i < context.ApiDescription.ActionDescriptor.Parameters.Count; i++)
            {
                if (context.ApiDescription.ActionDescriptor.Parameters[i].ParameterType == typeof(IFormFile))
                {
                    var parameter = operation.Parameters.FirstOrDefault(p => p.Name.Equals(operation.Parameters[i].Name, StringComparison.OrdinalIgnoreCase));
                    if (parameter == null) return;

                    // remove o parâmetro
                    operation.Parameters.Remove(parameter);

                    // insere o novo parâmetro modificado
                    var fileParam = new NonBodyParameter
                    {
                        Type = "file",
                        In = "formData",
                        Description = parameter.Description,
                        Name = parameter.Name,
                        Required = parameter.Required,
                    };
                    operation.Parameters.Insert(i, fileParam);
                    operation.Consumes.Add("multipart/form-data");
                }
            }
        }
    }
}