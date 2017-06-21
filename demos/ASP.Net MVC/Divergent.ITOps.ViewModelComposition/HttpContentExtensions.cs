using Newtonsoft.Json;
using System.Dynamic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Divergent.ITOps.ViewModelComposition
{
    public static class HttpContentExtensions
    {
        public static async Task<ExpandoObject> AsExpandoAsync(this HttpContent content)
        {
            var result = await content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<ExpandoObject>(result, CamelCaseToPascalSettings.GetSerializerSettings());
        }

        public static async Task<ExpandoObject[]> AsExpandoArrayAsync(this HttpContent content)
        {
            var result = await content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<ExpandoObject[]>(result, CamelCaseToPascalSettings.GetSerializerSettings());
        }
    }
}