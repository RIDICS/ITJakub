using System;
using System.Collections.Generic;
using System.Linq;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Vokabular.Shared.AspNetCore.WebApiUtils.Documentation
{
    public class PolymorphismSchemaFilter<TBase> : ISchemaFilter
    {
        private readonly Lazy<HashSet<Type>> m_derivedTypes = new Lazy<HashSet<Type>>(Init);

        private static HashSet<Type> Init()
        {
            var abstractType = typeof(TBase);
            var dTypes = abstractType.Assembly
                .GetTypes()
                .Where(x => abstractType != x && abstractType.IsAssignableFrom(x));

            var result = new HashSet<Type>();

            foreach (var item in dTypes)
                result.Add(item);

            return result;
        }

        public void Apply(Schema schema, SchemaFilterContext context)
        {
            if (!m_derivedTypes.Value.Contains(context.SystemType)) return;

            var clonedSchema = new Schema
            {
                Properties = schema.Properties,
                Type = schema.Type,
                Required = schema.Required
            };

            //schemaRegistry.Definitions[typeof(T).Name]; does not work correctly in SwashBuckle
            var parentSchema = new Schema { Ref = "#/definitions/" + typeof(TBase).Name };

            schema.AllOf = new List<Schema> { parentSchema, clonedSchema };

            //reset properties for they are included in allOf, should be null but code does not handle it
            schema.Properties = new Dictionary<string, Schema>();
        }
    }
}