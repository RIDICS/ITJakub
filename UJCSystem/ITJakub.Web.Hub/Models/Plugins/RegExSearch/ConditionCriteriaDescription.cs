using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace ITJakub.Web.Hub.Models.Plugins.RegExSearch
{
    [JsonConverter(typeof(ConditionCriteriaDescriptionConverter))]
    public class ConditionCriteriaDescription
    {
        public int SearchType { get; set; }
        public int ConditionType { get; set; }
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


    public class ConditionCriteriaDescriptionConverter : CustomCreationConverter<ConditionCriteriaDescription>
    {
        public ConditionCriteriaDescription Create(Type objectType, JObject jObject)
        {
            var type = (string) jObject.Property("conditionType");
            ConditionTypeEnum conditionTypeEnum;

            if (Enum.TryParse(type, out conditionTypeEnum))
            {
                switch (conditionTypeEnum)
                {
                    case ConditionTypeEnum.DatingList:
                        return new DatingListCriteriaDescription();
                    case ConditionTypeEnum.WordList:
                        return new WordListCriteriaDescription();
                }
            }

            throw new ApplicationException(string.Format("The type {0} is not supported!", type));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            var target = Create(objectType, jObject);
            serializer.Populate(jObject.CreateReader(), target);
            return target;
        }

        public override ConditionCriteriaDescription Create(Type objectType)
        {
            throw new NotImplementedException();
        }
    }
}