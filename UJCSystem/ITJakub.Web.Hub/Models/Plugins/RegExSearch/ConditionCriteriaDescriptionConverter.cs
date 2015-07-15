using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ITJakub.Web.Hub.Models.Plugins.RegExSearch
{
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
                        break;
                    case ConditionTypeEnum.WordList:
                        var wordList = new WordListCriteriaDescription();
                        var wordDescriptions = conditions.ToObject<IList<WordCriteriaDescription>>(serializer);
                        wordList.WordCriteriaDescription = wordDescriptions;
                        result = wordList;
                        break;
                }

                if (result != null)
                {
                    result.ConditionType = conditionTypeEnum;
                    var searchType = (int)jObject["searchType"];
                    result.SearchType = searchType;
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