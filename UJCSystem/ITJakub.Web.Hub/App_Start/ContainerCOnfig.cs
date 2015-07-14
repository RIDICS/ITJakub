using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using ITJakub.Web.Hub.Models.Plugins.RegExSearch;
using Newtonsoft.Json;

namespace ITJakub.Web.Hub
{
    public class ContainerConfig
    {
        public static void InitializeContainers()
        {
            var container = Container.Current;

            //ValueProviderFactories.Factories.Remove(ValueProviderFactories.Factories.OfType<JsonValueProviderFactory>().FirstOrDefault());
            //ValueProviderFactories.Factories.Add(new JsonNetValueProviderFactory());
        

            //ModelBinders.Binders.Add(typeof(ConditionCriteriaDescription), new ConditionCriteriaDescriptionCustomBinder());

            //var serializerSettings = GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings;
            //serializerSettings.Converters.Add(new ConditionCriteriaDescriptionConverter());
            //serializerSettings.TypeNameHandling = TypeNameHandling.None;

            //JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            //{
            //    Formatting = Formatting.Indented,
            //    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            //    Converters = new List<JsonConverter> {new ConditionCriteriaDescriptionConverter() },
            //};

        }
    }
}