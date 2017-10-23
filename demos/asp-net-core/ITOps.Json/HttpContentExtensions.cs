using Newtonsoft.Json;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ITOps.Json
{
    public static class HttpContentExtensions
    {
        private static JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new PascalCaseExpandoObjectConverter() }
        };

        public static async Task<ExpandoObject> AsExpandoAsync(this HttpContent content)
            => JsonConvert.DeserializeObject<ExpandoObject>(await content.ReadAsStringAsync(), serializerSettings);

        public static async Task<ExpandoObject[]> AsExpandoArrayAsync(this HttpContent content)
            => JsonConvert.DeserializeObject<ExpandoObject[]>(await content.ReadAsStringAsync(), serializerSettings);
    }
}
