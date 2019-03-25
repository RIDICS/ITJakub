using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Vokabular.MainService.DataContracts.Contracts.Permission;

namespace Vokabular.MainService.DataContracts.Utils
{
    public class SpecialPermissionJsonConverter : JsonConverter
    {
        private bool m_canRead;

        public SpecialPermissionJsonConverter()
        {
            m_canRead = true;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new InvalidOperationException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);

            if (CanConvert(objectType))
            {
                var keyProperty = jObject.Property("key");
                var keyString = keyProperty.Value.Value<string>();
                var key = (SpecialPermissionTypeContract)Enum.Parse(typeof(SpecialPermissionTypeContract), keyString, true);

                var resultObjectType = GetResultObjectType(key);
                var resultObject = jObject.ToObject(resultObjectType, serializer);
                
                return resultObject;
            }
            else
            {
                m_canRead = false; // Derived type is deserialized normally
                var resultObject = jObject.ToObject(objectType, serializer);
                m_canRead = true;

                return resultObject;
            }
        }

        private Type GetResultObjectType(SpecialPermissionTypeContract key)
        {
            switch (key)
            {
                case SpecialPermissionTypeContract.News:
                    return typeof(NewsPermissionContract);
                case SpecialPermissionTypeContract.UploadBook:
                    return typeof(UploadBookPermissionContract);
                case SpecialPermissionTypeContract.Permissions:
                    return typeof(ManagePermissionsPermissionContract);
                case SpecialPermissionTypeContract.Feedback:
                    return typeof(FeedbackPermissionContract);
                case SpecialPermissionTypeContract.CardFile:
                    return typeof(CardFilePermissionContract);
                case SpecialPermissionTypeContract.AutoImport:
                    return typeof(AutoImportCategoryPermissionContract);
                case SpecialPermissionTypeContract.ReadLemmatization:
                    return typeof(ReadLemmatizationPermissionContract);
                case SpecialPermissionTypeContract.EditLemmatization:
                    return typeof(EditLemmatizationPermissionContract);
                case SpecialPermissionTypeContract.DerivateLemmatization:
                    return typeof(DerivateLemmatizationPermissionContract);
                case SpecialPermissionTypeContract.EditionPrint:
                    return typeof(EditionPrintPermissionContract);
                case SpecialPermissionTypeContract.EditStaticText:
                    return typeof(EditStaticTextPermissionContract);
                case SpecialPermissionTypeContract.ManagerRepositoryImport:
                    return typeof(ManageRepositoryImportPermissionContract);
                default:
                    throw new ArgumentOutOfRangeException(nameof(key), key, null);
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(SpecialPermissionContract);
        }

        public override bool CanWrite => false;

        public override bool CanRead => m_canRead;
    }
}