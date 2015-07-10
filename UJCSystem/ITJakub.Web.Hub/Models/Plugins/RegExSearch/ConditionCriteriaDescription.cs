using System;
using System.Collections.Generic;
using Newtonsoft.Json;
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
            return (objectType == typeof(ConditionCriteriaDescription));
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

}