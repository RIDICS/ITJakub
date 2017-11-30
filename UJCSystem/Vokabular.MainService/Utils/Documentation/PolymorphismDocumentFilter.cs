using System;
using System.Collections.Generic;
using System.Linq;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Vokabular.MainService.Utils.Documentation
{
    public class PolymorphismDocumentFilter<TBase> : IDocumentFilter
    {
        private readonly SchemaIdManager m_schemaIdManager;

        public PolymorphismDocumentFilter()
        {
            var settings = new SchemaRegistrySettings(); // warning: getting settings directly from Swagger is't possible
            m_schemaIdManager = new SchemaIdManager(settings.SchemaIdSelector);
        }

        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            RegisterSubClasses(context.SchemaRegistry, typeof(TBase));
        }

        private void RegisterSubClasses(ISchemaRegistry schemaRegistry, Type abstractType)
        {
            const string discriminatorName = "key";
            
            var parentSchema = schemaRegistry.Definitions[m_schemaIdManager.IdFor(abstractType)];

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