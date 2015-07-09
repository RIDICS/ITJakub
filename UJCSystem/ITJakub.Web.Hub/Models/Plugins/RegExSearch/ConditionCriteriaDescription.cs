using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ITJakub.Web.Hub.Models.Plugins.RegExSearch
{
    [JsonConverter(typeof(ConditionCriteriaWrapperConverter))]
    public class ConditonCriteriaDescriptionWrapper
    {
        public ConditionTypeEnum ConditionType { get; set; }
        public ConditionCriteriaDescription ConditionDescription { get; set; }
    }

    public class ConditionCriteriaDescription
    {
        public int SearchType { get; set; }
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


    public class ConditionCriteriaWrapperConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(ConditonCriteriaDescriptionWrapper));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var result = new ConditonCriteriaDescriptionWrapper();
            JObject jObject = JObject.Load(reader);
            var type = (string)jObject["conditionType"];
            ConditionTypeEnum conditionTypeEnum;

            if (Enum.TryParse(type, out conditionTypeEnum))
            {
                result.ConditionType = conditionTypeEnum;
                var conditonDescription = jObject["conditionDescription"]; //TODO add to json
                switch (conditionTypeEnum)
                {
                    case ConditionTypeEnum.DatingList:
                        result.ConditionDescription = conditonDescription.ToObject<DatingListCriteriaDescription>(serializer);
                        //return new DatingListCriteriaDescription();
                        break;
                    case ConditionTypeEnum.WordList:
                        result.ConditionDescription = conditonDescription.ToObject<WordListCriteriaDescription>(serializer);
                        //return new WordListCriteriaDescription();
                        break;
                }
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

}