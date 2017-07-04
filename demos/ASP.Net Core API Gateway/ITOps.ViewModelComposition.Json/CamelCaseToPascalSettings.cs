using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace ITOps.ViewModelComposition.Json
{
    public class CamelCaseToPascalSettings
    {
        static JsonSerializerSettings settings;

        static CamelCaseToPascalSettings()
        {
            settings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new List<JsonConverter> { new CamelCaseToPascalCaseExpandoObjectConverter() }
            };
        }

        public static JsonSerializerSettings GetSerializerSettings()
        {
            return settings;
        }
    }
}
