using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Vokabular.RestClient.Headers;

namespace Vokabular.RestClient.Extensions
{
    public static class HttpRestClientExtensions
    {
        private static JsonSerializerSettings CreateJsonSerializerSettings()
        {
            return new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
            };
        }

        private static JsonSerializer CreateJsonSerializer()
        {
            var settings = CreateJsonSerializerSettings();
            return JsonSerializer.Create(settings);
        }

        public static async Task<T> ReadAsAsync<T>(this HttpContent content)
        {
            using (var stream = await content.ReadAsStreamAsync())
            using (var textReader = new StreamReader(stream))
            {
                var contentString = await textReader.ReadToEndAsync();

                using (var stringReader = new StringReader(contentString))
                using (var jsonReader = new JsonTextReader(stringReader))
                {
                    var serializer = CreateJsonSerializer();
                    var item = serializer.Deserialize<T>(jsonReader);

                    return item;
                }
            }
        }

        public static T Deserialize<T>(this string content)
        {
            var settings = CreateJsonSerializerSettings();
            var item = JsonConvert.DeserializeObject<T>(content, settings);
            return item;
        }

        public static async Task<T> ReadXmlAsAsync<T>(this HttpContent content)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            using (var stream = await content.ReadAsStreamAsync())
            {
                var item = serializer.Deserialize(stream);
                return (T) item;
            }
        }

        public static async Task<HttpResponseMessage> SendAsJsonAsync(this HttpClient httpClient, HttpRequestMessage requestMessage, object value)
        {
            var stream = new MemoryStream(); // Using block isn't used. Stream is disposed automatically by httpClient.SendAsync method.
            var textWriter = new StreamWriter(stream);
            var jsonWriter = new JsonTextWriter(textWriter);

            var serializer = CreateJsonSerializer();
            serializer.Serialize(jsonWriter, value);

            await jsonWriter.FlushAsync();
            stream.Seek(0, SeekOrigin.Begin);

            requestMessage.Content = new StreamContent(stream);
            requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(ContentTypes.JsonContentType);

            var result = await httpClient.SendAsync(requestMessage);
            return result;
        }

        public static async Task<HttpResponseMessage> SendAsWwwFormUrlEncodedAsync(this HttpClient httpClient,
            HttpRequestMessage requestMessage, IEnumerable<KeyValuePair<string, string>> formData)
        {
            var sb = new StringBuilder();
            foreach (var dataItem in formData)
            {
                if (sb.Length > 0)
                {
                    sb.Append("&");
                }

                sb.AppendFormat("{0}={1}", dataItem.Key.EncodeQueryString(), dataItem.Value?.EncodeQueryString());
            }

            requestMessage.Content = new StringContent(sb.ToString(), Encoding.UTF8, ContentTypes.WwwFormUrlEncodedContentType);

            var result = await httpClient.SendAsync(requestMessage);
            return result;
        }
    }
}