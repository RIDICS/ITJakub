using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.Shared.DataContracts.Utils
{
    public class SearchCriteriaJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var resultList = new List<SearchCriteriaContract>();
            var jArray = serializer.Deserialize<JArray>(reader);

            foreach (var jToken in jArray)
            {
                var jObject = (JObject) jToken;

                var keyProperty = jObject.Property("key");
                var criteriaKeyString = keyProperty.Value.Value<string>();
                var criteriaKey = (CriteriaKey) Enum.Parse(typeof(CriteriaKey), criteriaKeyString, true);
                
                var criteriaObjectType = GetCriteriaType(criteriaKey);
                var criteriaObject = (SearchCriteriaContract) jObject.ToObject(criteriaObjectType, serializer);
                resultList.Add(criteriaObject);
            }
            
            return resultList;
        }

        private Type GetCriteriaType(CriteriaKey criteriaKey)
        {
            switch (criteriaKey)
            {
                case CriteriaKey.Author:
                case CriteriaKey.Title:
                case CriteriaKey.Editor:
                case CriteriaKey.Fulltext:
                case CriteriaKey.Heading:
                case CriteriaKey.Sentence:
                case CriteriaKey.Headword:
                case CriteriaKey.HeadwordDescription:
                case CriteriaKey.Term:
                    return typeof(WordListCriteriaContract);
                case CriteriaKey.Dating:
                    return typeof(DatingListCriteriaContract);
                case CriteriaKey.Result:
                    return typeof(ResultCriteriaContract);
                case CriteriaKey.ResultRestriction:
                    return typeof(ResultRestrictionCriteriaContract);
                case CriteriaKey.SnapshotResultRestriction:
                    return typeof(SnapshotResultRestrictionCriteriaContract);
                case CriteriaKey.TokenDistance:
                case CriteriaKey.HeadwordDescriptionTokenDistance:
                    return typeof(TokenDistanceListCriteriaContract);
                case CriteriaKey.SelectedCategory:
                    return typeof(SelectedCategoryCriteriaContract);
                case CriteriaKey.Authorization:
                    return typeof(AuthorizationCriteriaContract);
                default:
                    throw new ArgumentOutOfRangeException(nameof(criteriaKey), criteriaKey, null);
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<SearchCriteriaContract>);
        }
    }
}
