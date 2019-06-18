using System;
using System.Collections.Generic;
using System.Linq;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Vokabular.Shared.AspNetCore.WebApiUtils.Documentation
{
    public class PolymorphismDocumentFilter<TBase> : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            RegisterSubClasses(context.SchemaRegistry, typeof(TBase));
        }

        private void RegisterSubClasses(ISchemaRegistry schemaRegistry, Type abstractType)
        {
            const string discriminatorName = "key";
            
            var parentSchema = schemaRegistry.Definitions[abstractType.Name];

            //set up a discriminator property (it must be required)
            parentSchema.Discriminator = discriminatorName;
            parentSchema.Required = new List<string> { discriminatorName };

            if (!parentSchema.Properties.ContainsKey(discriminatorName))
                parentSchema.Properties.Add(discriminatorName, new Schema { Type = "string" });

            //register all subclasses
            var derivedTypes = abstractType.Assembly
                .GetTypes()
                .Where(x => abstractType != x && abstractType.IsAssignableFrom(x));

            foreach (var item in derivedTypes)
                schemaRegistry.GetOrRegister(item);
        }
    }
}