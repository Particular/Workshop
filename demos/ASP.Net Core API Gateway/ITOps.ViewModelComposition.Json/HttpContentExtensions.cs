using Newtonsoft.Json;
using System.Dynamic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ITOps.ViewModelComposition.Json
{
    public static class HttpContentExtensions
    {
        public static async Task<ExpandoObject> AsExpandoAsync(this HttpContent content) 
            => JsonConvert.DeserializeObject<ExpandoObject>(await content.ReadAsStringAsync(), CamelCaseToPascalSettings.GetSerializerSettings());
        
        public static async Task<ExpandoObject[]> AsExpandoArrayAsync(this HttpContent content) 
            => JsonConvert.DeserializeObject<ExpandoObject[]>(await content.ReadAsStringAsync(), CamelCaseToPascalSettings.GetSerializerSettings());
    }
}