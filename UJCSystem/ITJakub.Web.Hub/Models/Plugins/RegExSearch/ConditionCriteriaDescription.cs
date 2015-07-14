using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace ITJakub.Web.Hub.Models.Plugins.RegExSearch
{
    [JsonConverter(typeof(ConditionCriteriaDescriptionConverter))]
    public class ConditionCriteriaDescription
    {
        public int SearchType { get; set; }

        public ConditionTypeEnum ConditionType { get; set; }

    }

    public class WordListCriteriaDescription : ConditionCriteriaDescription
    {
        public IList<WordCriteriaDescription> WordCriteriaDescription { get; set; }
    }

    public class WordCriteriaDescription
    {
        public string StartsWith { get; set; }
        public IList<string> Contains { get; set; }
        public string EndsWith { get; set; }
    }


    public class DatingListCriteriaDescription : ConditionCriteriaDescription
    {
        public IList<DatingCriteriaDescription> DatingCriteriaDescription { get; set; }
    }

    public class DatingCriteriaDescription
    {
        public int? NotAfter { get; set; }
        public int? NotBefore { get; set; }
    }

    public enum ConditionTypeEnum
    {
        WordList = 0,
        DatingList = 1
    }


    public class ConditionCriteriaDescriptionConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            var canConvert = (objectType == typeof(ConditionCriteriaDescription));
            return canConvert;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);
            var type = (string)jObject["conditionType"];
            ConditionTypeEnum conditionTypeEnum;
            ConditionCriteriaDescription result = null;

            if (Enum.TryParse(type, out conditionTypeEnum))
            {
                
                var conditions = jObject["conditions"];
                switch (conditionTypeEnum)
                {
                    case ConditionTypeEnum.DatingList:
                        var datingList = new DatingListCriteriaDescription();
                        var list = conditions.ToObject<IList<DatingCriteriaDescription>>(serializer);
                        datingList.DatingCriteriaDescription = list;
                        result = datingList;
                        //return new DatingListCriteriaDescription();
                        break;
                    case ConditionTypeEnum.WordList:
                        var wordList = new WordListCriteriaDescription();
                        var wordDescriptions = conditions.ToObject<IList<WordCriteriaDescription>>(serializer);
                        wordList.WordCriteriaDescription = wordDescriptions;
                        result = wordList;
                        //return new WordListCriteriaDescription();
                        break;
                }
                var searchType = (int)jObject["searchType"];
                if (result != null) result.SearchType = searchType;
            }

            return result;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }


    //TODO not used (is for model binder)
    public class ConditionCriteriaDescriptionCustomBinder : DefaultModelBinder
    {

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType == typeof(ConditionCriteriaDescription))
            {
                var requestInputStream = controllerContext.HttpContext.Request.InputStream;
                if(requestInputStream.CanSeek)requestInputStream.Seek(0, SeekOrigin.Begin);
                StreamReader reader = new StreamReader(requestInputStream);
                string json = reader.ReadToEnd();
                var conditionDescriptionModel = JsonConvert.DeserializeObject<IEnumerable<ConditionCriteriaDescription>>(json, new ConditionCriteriaDescriptionConverter());
                return conditionDescriptionModel;
            }
            return base.BindModel(controllerContext, bindingContext);
        }
    }

    //TODO not used (is replacement for value factory for json)
    public class JsonNetValueProviderFactory : ValueProviderFactory
    {
        public override IValueProvider GetValueProvider(ControllerContext controllerContext)
        {
            // first make sure we have a valid context
            if (controllerContext == null)
                throw new ArgumentNullException("controllerContext");

            // now make sure we are dealing with a json request
            if (!controllerContext.HttpContext.Request.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
                return null;

            // get a generic stream reader (get reader for the http stream)
            var inputStream = controllerContext.HttpContext.Request.InputStream;
            if (inputStream.CanSeek) inputStream.Seek(0, SeekOrigin.Begin);
            StreamReader streamReader = new StreamReader(inputStream);
            // convert stream reader to a JSON Text Reader
            JsonTextReader JSONReader = new JsonTextReader(streamReader);
            // tell JSON to read
            if (!JSONReader.Read())
                return null;

            // make a new Json serializer
            JsonSerializer JSONSerializer = new JsonSerializer();
            // add the dyamic object converter to our serializer
            JSONSerializer.Converters.Add(new ExpandoObjectConverter());

            // use JSON.NET to deserialize object to a dynamic (expando) object
            Object JSONObject;
            // if we start with a "[", treat this as an array
            if (JSONReader.TokenType == JsonToken.StartArray)
                JSONObject = JSONSerializer.Deserialize<List<ExpandoObject>>(JSONReader);
            else
                JSONObject = JSONSerializer.Deserialize<ExpandoObject>(JSONReader);

            // create a backing store to hold all properties for this deserialization
            Dictionary<string, object> backingStore = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            // add all properties to this backing store
            AddToBackingStore(backingStore, String.Empty, JSONObject);
            // return the object in a dictionary value provider so the MVC understands it
            return new DictionaryValueProvider<object>(backingStore, CultureInfo.CurrentCulture);
        }

        private static void AddToBackingStore(Dictionary<string, object> backingStore, string prefix, object value)
        {
            IDictionary<string, object> d = value as IDictionary<string, object>;
            if (d != null)
            {
                foreach (KeyValuePair<string, object> entry in d)
                {
                    AddToBackingStore(backingStore, MakePropertyKey(prefix, entry.Key), entry.Value);
                }
                return;
            }

            IList l = value as IList;
            if (l != null)
            {
                for (int i = 0; i < l.Count; i++)
                {
                    AddToBackingStore(backingStore, MakeArrayKey(prefix, i), l[i]);
                }
                return;
            }

            // primitive
            backingStore[prefix] = value;
        }

        private static string MakeArrayKey(string prefix, int index)
        {
            return prefix + "[" + index.ToString(CultureInfo.InvariantCulture) + "]";
        }

        private static string MakePropertyKey(string prefix, string propertyName)
        {
            return (String.IsNullOrEmpty(prefix)) ? propertyName : prefix + "." + propertyName;
        }
    }

}